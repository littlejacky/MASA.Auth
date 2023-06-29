﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
GlobalValidationOptions.SetDefaultCulture("zh-CN");

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.Auth, MasaStackApp.Service, true);
var masaStackConfig = builder.Services.GetMasaStackConfig();

var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();

var identityServerUrl = masaStackConfig.GetSsoDomain();

#if DEBUG
identityServerUrl = "http://localhost:18200";
//builder.Services.AddDaprStarter(opt =>
//{
//    opt.DaprHttpPort = 3600;
//    opt.DaprGrpcPort = 3601;
//});
#endif

builder.Services.AddAutoInject();
builder.Services.AddDaprClient();

builder.Services.AddObjectStorage(option => option.UseAliyunStorage());

builder.Services.AddObservable(builder.Logging, () => new MasaObservableOptions
{
    ServiceNameSpace = builder.Environment.EnvironmentName,
    ServiceVersion = masaStackConfig.Version,
    ServiceName = masaStackConfig.GetServiceId(MasaStackProject.Auth),
    Layer = masaStackConfig.Namespace,
    ServiceInstanceId = builder.Configuration.GetValue<string>("HOSTNAME")
}, () => masaStackConfig.OtlpUrl);

builder.Services.AddMasaIdentity(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
    options.Mapping(nameof(MasaUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
    options.Mapping(nameof(MasaUser.StaffId), IdentityClaimConsts.STAFF);
    options.Mapping(nameof(MasaUser.Account), IdentityClaimConsts.ACCOUNT);
});
builder.Services
    .AddScoped<IAuthorizationMiddlewareResultHandler, CodeAuthorizationMiddlewareResultHandler>()
    .AddSingleton<IAuthorizationHandler, DefaultRuleCodeAuthorizationHandler>()
    .AddSingleton<IAuthorizationPolicyProvider, DefaultRuleCodePolicyProvider>()
    .AddAuthorization(options =>
    {
        var defaultPolicy = new AuthorizationPolicyBuilder()
            // Remove if you don't need the user to be authenticated
            .RequireAuthenticatedUser()
            .AddRequirements(new DefaultRuleCodeRequirement(masaStackConfig.GetServiceId(MasaStackProject.Auth)))
            .Build();
        options.DefaultPolicy = defaultPolicy;
    })
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = identityServerUrl;
        options.RequireHttpsMetadata = false;
        //options.Audience = "";
        options.TokenValidationParameters.ValidateAudience = false;
        options.MapInboundClaims = false;

        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (
                sender,
                certificate,
                chain,
                sslPolicyErrors) =>
            { return true; }
        };
    });

builder.Services.AddI18n(Path.Combine("Assets", "I18n"));

MapsterAdapterConfig.TypeAdapter();

var redisOption = new RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions> {
        new RedisServerOptions()
        {
            Host= masaStackConfig.RedisModel.RedisHost,
            Port= masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword
};
builder.Services.AddMultilevelCache(options => options.UseStackExchangeRedisCache());
builder.Services.AddAuthClientMultilevelCache(redisOption);
builder.Services.AddDccClient(redisOption);
builder.Services
            .AddPmClient(masaStackConfig.GetPmServiceDomain())
            .AddSchedulerClient(masaStackConfig.GetSchedulerServiceDomain())
            .AddMcClient(masaStackConfig.GetMcServiceDomain())
            .AddLadpContext()
            .AddElasticsearchAutoComplete();
//todo when scheduler is unready, this code should not run
await builder.Services.AddSchedulerJobAsync();

builder.Services
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
.AddEndpointsApiExplorer()
.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.OrderActionsBy(o => o.GroupName);
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MASA Auth API",
        Version = "v1",
        Description = "",
        TermsOfService = new Uri("https://www.masastack.com"),
        Contact = new OpenApiContact
        {
            Name = "MASA Stack",
            Url = new Uri("https://www.masastack.com/aboutus#aboutus_contactus")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xxxxxxxxxxxxxxx\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
})
.AddValidatorsFromAssembly(Assembly.GetEntryAssembly())
.AddValidatorsFromAssemblies(new Assembly[] { typeof(PasswordValidator).Assembly, typeof(ResetUserPasswordValidator).Assembly })
.AddDomainEventBus(dispatcherOptions =>
{
    dispatcherOptions
    .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<AuthDbContext>())
    .UseEventBus(eventBusBuilder =>
    {
        eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
    })
    .UseUoW<AuthDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer();
        dbOptions.UseFilter();
    })
    .UseRepository<AuthDbContext>();
});

await builder.Services.AddStackIsolationAsync(MasaStackProject.Auth.Name);

builder.Services.AddStackMiddleware();

await builder.MigrateDbContextAsync<AuthDbContext>((context, services) =>
{
    //todo split
    //await new AuthSeedData().SeedAsync(context, services);
    return Task.CompletedTask;
});
builder.Services.AddOidcCache(publicConfiguration);
await builder.Services.AddOidcDbContext<AuthDbContext>(async option =>
{
    await new AuthSeedData().SeedAsync(builder);

    await option.SeedStandardResourcesAsync();
    await option.SyncCacheAsync();
});

builder.Services.RemoveAll(typeof(IProcessor));

var app = builder.AddServices(options =>
{
    options.DisableAutoMapRoute = true; // todo :remove it before v1.0
    options.GetPrefixes = new() { "Get", "Select", "Find" };
    options.PostPrefixes = new() { "Post", "Add", "Create", "Send" };
});

app.UseI18n();

app.UseMasaExceptionHandler(opt =>
{
    opt.ExceptionHandler = context =>
    {
        if (context.Exception is ValidationException validationException)
        {
            context.ToResult(validationException.Errors.Select(err => err.ToString()).FirstOrDefault()!);
        }
        else if (context.Exception is UserStatusException userStatusException)
        {
            context.ToResult(userStatusException.Message, (int)MasaAuthHttpStatusCode.UserStatusException);
        }
    };
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        var path = Path.Combine(app.Environment.WebRootPath, "swagger/ui/index.html");
        if (File.Exists(path))
        {
            options.IndexStream = () => new MemoryStream(File.ReadAllBytes(path));
        }
    });
    app.UseMiddleware<SwaggerAuthentication>();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<MasaAuthorizeMiddleware>();
app.UseStackIsolation();
app.UseStackMiddleware();

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
});
app.UseHttpsRedirection();

app.Run();