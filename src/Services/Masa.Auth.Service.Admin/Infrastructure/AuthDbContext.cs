﻿namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContext : IsolationDbContext
{
    public const string PERMISSION_SCHEMA = "permissions";
    public const string SUBJECT_SCHEMA = "subjects";
    public const string ORGANIZATION_SCHEMA = "organizations";
    public const string SSO_SCHEMA = "sso";
    public const string PROJECTS_SCHEMA = "projects";

    public AuthDbContext(MasaDbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreatingExecuting(builder);
    }
}