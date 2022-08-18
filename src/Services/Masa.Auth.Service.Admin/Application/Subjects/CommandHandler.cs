﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly AuthDbContext _authDbContext;
    readonly StaffDomainService _staffDomainService;
    readonly UserDomainService _userDomainService;
    readonly ThirdPartyUserDomainService _thirdPartyUserDomainService;
    readonly IUserContext _userContext;
    readonly IDistributedCacheClient _cache;
    readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;
    readonly ILdapFactory _ldapFactory;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILogger<CommandHandler> _logger;
    readonly IEventBus _eventBus;

    public CommandHandler(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        IStaffRepository staffRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        IThirdPartyUserRepository thirdPartyUserRepository,
        AuthDbContext authDbContext,
        StaffDomainService staffDomainService,
        UserDomainService userDomainService,
        ThirdPartyUserDomainService thirdPartyUserDomainService,
        IDistributedCacheClient cache,
        IUserContext userContext,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository,
        ILdapFactory ldapFactory,
        ILdapIdpRepository ldapIdpRepository,
        ILogger<CommandHandler> logger,
        IEventBus eventBus)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _staffRepository = staffRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _authDbContext = authDbContext;
        _staffDomainService = staffDomainService;
        _userDomainService = userDomainService;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
        _cache = cache;
        _userContext = userContext;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
        _ldapFactory = ldapFactory;
        _ldapIdpRepository = ldapIdpRepository;
        _logger = logger;
        _eventBus = eventBus;
    }

    #region User

    [EventHandler(1)]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        var user = await VerifyUserRepeatAsync(default, userDto.PhoneNumber, userDto.Email, userDto.IdCard, userDto.Account, !command.WhenExisReturn);
        if (user is not null)
        {
            command.NewUser = user;
            return;
        }
        user = new User(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.Department, userDto.Position, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Gender);
        user.AddRoles(userDto.Roles.ToArray());
        user.AddPermissions(userDto.Permissions);
        await AddUserAsync(user);
        command.NewUser = user;
    }

    async Task AddUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await _userDomainService.AddAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        await VerifyUserRepeatAsync(userDto.Id, userDto.PhoneNumber, userDto.Email, userDto.IdCard, default);
        user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        var user = await _userRepository.GetDetailAsync(command.User.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        if (user.IsAdmin())
        {
            throw new UserFriendlyException("超级管理员 无法删除");
        }
        if (user.Id == _userContext.GetUserId<Guid>())
        {
            throw new UserFriendlyException("当前用户不能删除自己");
        }

        await _userRepository.RemoveAsync(user);
        await _userDomainService.RemoveAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.GetDetailAsync(userDto.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        var roles = user.Roles.Select(role => role.RoleId).Union(userDto.Roles);
        user.AddRoles(userDto.Roles.ToArray());
        user.AddPermissions(userDto.Permissions);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAuthorizationAsync(roles);
    }

    [EventHandler(1)]
    public async Task ResetUserPasswordAsync(ResetUserPasswordCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        user.UpdatePassword(userDto.Password);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task UpdateUserPasswordAsync(UpdateUserPasswordCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserExistAsync(userModel.Id);
        if (user.VerifyPassword(userModel.OldPassword) is false)
        {
            throw new UserFriendlyException("password verification failed");
        }
        user.UpdatePassword(userModel.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserBasicInfoAsync(UpdateUserBasicInfoCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserExistAsync(userModel.Id);
        user.UpdateBasicInfo(userModel.DisplayName, userModel.Gender);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task UpsertUserAsync(UpsertUserCommand command)
    {
        var userModel = command.User;
        var user = default(User);
        if (userModel.Id != default)
        {
            user = await _userRepository.FindAsync(u => u.Id == userModel.Id);
            if (user is not null)
            {
                await VerifyUserRepeatAsync(userModel.Id, userModel.PhoneNumber, default, userModel.Email, userModel.IdCard, default);
                user.Update(userModel.Name, userModel.DisplayName!, userModel.IdCard, userModel.CompanyName, userModel.Gender);
                await _userRepository.UpdateAsync(user);
                await _userDomainService.UpdateAsync(user);
                command.NewUser = user.Adapt<UserModel>();
                return;
            }
        }
        user = new User(userModel.Name, userModel.DisplayName, default, userModel.IdCard, userModel.Account, default, userModel.CompanyName, default, default, true, userModel.PhoneNumber, default, userModel.Email, default, userModel.Gender);
        await AddUserAsync(user);
        command.NewUser = user.Adapt<UserModel>(); ;
    }

    [EventHandler(1)]
    public async Task DisableUserAsync(DisableUserCommand command)
    {
        var userModel = command.User;
        var user = await _userRepository.FindAsync(u => u.Account == userModel.Account);
        if (user is null)
            throw new UserFriendlyException($"User with account {userModel.Account} does not exist");

        user.Disabled();
        await _userRepository.UpdateAsync(user);
        command.Result = true;
    }

    [EventHandler]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        //todo UserDomainService
        var account = validateByAccountCommand.UserAccountValidateDto.Account;
        var password = validateByAccountCommand.UserAccountValidateDto.Password;
        var key = CacheKey.AccountLoginKey(account);
        var loginCache = await _cache.GetAsync<CacheLogin>(key);
        if (loginCache is not null && loginCache.LoginErrorCount >= 5)
        {
            throw new UserFriendlyException("您连续输错密码5次,登录已冻结，请三十分钟后再次尝试");
        }

        var isLdap = validateByAccountCommand.UserAccountValidateDto.IsLdap;
        if (isLdap)
        {
            var ldaps = await _ldapIdpRepository.GetListAsync();
            if (!ldaps.Any())
            {
                throw new UserFriendlyException("没有配置LDAP认证");
            }
            if (ldaps.Count() > 1)
            {
                _logger.LogWarning("存在多个Ldap配置,域账号登录时只使用第一个配置");
            }
            var ldap = ldaps.First();
            var ldapOptions = ldap.Adapt<LdapOptions>();
            var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);

            var ldapUser = await ldapProvider.GetUserByUserNameAsync(account);
            if (ldapUser == null)
            {
                throw new UserFriendlyException("域账号不存在");
            }

            var dc = new Regex("(?<=DC=).+(?=,)").Match(ldapOptions.BaseDn).Value;
            if (!await ldapProvider.AuthenticateAsync($"{dc}\\{account}", password))
            {
                throw new UserFriendlyException("域账号验证失败");
            }

            var upsertThirdPartyUserCommand = new UpsertThirdPartyUserForLdapCommand(ldap.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser), ldapUser.Name, ldapUser.DisplayName, ldapUser.Phone, ldapUser.EmailAddress, ldapUser.SamAccountName, password, ldapUser.Phone);
            await _eventBus.PublishAsync(upsertThirdPartyUserCommand);
        }

        var user = await _userRepository.FindAsync(u => u.Account == account);
        if (user != null)
        {
            if (!user.Enabled)
            {
                throw new UserFriendlyException("账号已禁用");
            }
            var success = user.VerifyPassword(password);
            if (!success)
            {
                loginCache ??= new() { FreezeTime = DateTimeOffset.Now.AddMinutes(30), Account = account };
                loginCache.LoginErrorCount++;
                var options = new CombinedCacheEntryOptions<CacheLogin>
                {
                    DistributedCacheEntryOptions = new()
                    {
                        AbsoluteExpiration = loginCache.FreezeTime
                    }
                };
                await _cache.SetAsync(key, loginCache, options);
                throw new UserFriendlyException("账号或密码错误");
            }

            if (loginCache is not null)
            {
                await _cache.RemoveAsync<CacheLogin>(key);
            }
            validateByAccountCommand.Result = success;
        }
    }

    [EventHandler(1)]
    public async Task VerifyUserRepeatAsync(VerifyUserRepeatCommand command)
    {
        var user = command.User;
        await VerifyUserRepeatAsync(user.Id, user.PhoneNumber, user.Email, user.IdCard, user.Account);
        command.Result = true;
    }

    [EventHandler]
    public async Task SyncUserAutoCompleteAsync(SyncUserAutoCompleteCommand command)
    {
        var users = await _userRepository.GetAllAsync();
        var syncCount = 0;
        while (syncCount < users.Count)
        {
            var syncUsers = users.Skip(syncCount)
                                .Take(command.Dto.OnceExecuteCount)
                                .Select(user => new UserSelectDto(user.Id, user.Name, user.DisplayName, user.Account, user.PhoneNumber, user.Email, user.Avatar));
            await _autoCompleteClient.SetBySpecifyDocumentAsync(syncUsers);
            syncCount += command.Dto.OnceExecuteCount;
        }
    }

    private async Task<User> CheckUserExistAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(u => u.Id == userId);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        return user;
    }

    private async Task<User?> VerifyUserRepeatAsync(Guid? userId, string? phoneNumber, string? email, string? idCard, string? account, bool throwException = true)
    {
        Expression<Func<User, bool>> condition = user => false;
        if (!string.IsNullOrEmpty(account))
            condition = condition.Or(user => user.Account == account);
        if (!string.IsNullOrEmpty(phoneNumber))
            condition = condition.Or(user => user.PhoneNumber == phoneNumber);
        if (!string.IsNullOrEmpty(email))
            condition = condition.Or(user => user.Email == email);
        if (!string.IsNullOrEmpty(idCard))
            condition = condition.Or(user => user.IdCard == idCard);
        if (userId is not null)
        {
            Expression<Func<User, bool>> condition2 = user => user.Id != userId;
            condition = condition2.And(condition);
        }

        var exitUser = await _userRepository.FindAsync(condition);
        if (exitUser is not null)
        {
            if (throwException is false) return exitUser;
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == exitUser.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {phoneNumber} already exists");
            if (string.IsNullOrEmpty(email) is false && email == exitUser.Email)
                throw new UserFriendlyException($"User with email {email} already exists");
            if (string.IsNullOrEmpty(idCard) is false && idCard == exitUser.IdCard)
                throw new UserFriendlyException($"User with idCard {idCard} already exists");
            if (string.IsNullOrEmpty(account) is false && account == exitUser.Account)
                throw new UserFriendlyException($"User with account {account} already exists");
        }
        return exitUser;
    }

    #endregion

    #region Staff

    [EventHandler]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        var staffDto = command.Staff;
        var staff = await VerifyStaffRepeatAsync(default, staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard, !command.WhenExisReturn);
        if (staff is not null) return;
        await AddStaffAsync(staffDto);
    }

    async Task AddStaffAsync(AddStaffDto staffDto)
    {
        var addUserDto = new AddUserDto(staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName, staffDto.Enabled, staffDto.PhoneNumber, default, staffDto.Email, staffDto.Address, default, staffDto.Position, default, staffDto.Password, staffDto.Gender, default, default);
        var addStaffBeforeEvent = new AddStaffBeforeDomainEvent(addUserDto, staffDto.Position);
        await _staffDomainService.AddBeforeAsync(addStaffBeforeEvent);
        var staff = new Staff(
                addStaffBeforeEvent.UserId,
                staffDto.Name,
                staffDto.DisplayName,
                staffDto.Avatar,
                staffDto.IdCard,
                staffDto.CompanyName,
                staffDto.Gender,
                staffDto.PhoneNumber,
                staffDto.Email,
                staffDto.JobNumber,
                addStaffBeforeEvent.PositionId,
                staffDto.StaffType,
                staffDto.Enabled,
                staffDto.Address
            );
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.AddAsync(staff);
        await _staffDomainService.AddAfterAsync(new(staff));
    }

    [EventHandler]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.Id == staffDto.Id);
        if (staff is null)
            throw new UserFriendlyException("This staff data does not exist");

        await VerifyStaffRepeatAsync(staffDto.Id, staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard);
        await UpdateStaffAsync(staff, staffDto);
    }

    async Task UpdateStaffAsync(Staff staff, UpdateStaffDto staffDto)
    {
        var updateStaffEvent = new UpdateStaffBeforeDomainEvent(staffDto.Position);
        await _staffDomainService.UpdateBeforeAsync(updateStaffEvent);

        staff.Update(
            updateStaffEvent.PositionId, staffDto.StaffType, staffDto.Enabled, staffDto.Name,
            staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName,
            staffDto.PhoneNumber, staffDto.Email, staffDto.Address, staffDto.Gender);
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        var teams = staff.TeamStaffs.Select(team => team.TeamId).Union(staffDto.Teams).Distinct().ToList();
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.UpdateAsync(staff);

        await _staffDomainService.UpdateAfterAsync(new(staff, teams));
    }

    [EventHandler]
    public async Task UpsertStaffAsync(UpsertStaffCommand command)
    {
        var staffDto = command.Staff;
        var staff = await VerifyStaffRepeatAsync(default, staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard, false);
        if (staff is not null)
        {
            var updateStaffDto = staffDto.Adapt<UpdateStaffDto>();
            updateStaffDto.Id = staff.Id;
            await UpdateStaffAsync(staff, updateStaffDto);
        }
        else
        {
            await AddStaffAsync(staffDto);
        }
    }

    [EventHandler]
    public async Task UpsertStaffForLdapAsync(UpsertStaffForLdapCommand command)
    {
        var staffDto = command.Staff;
        var staff = await VerifyStaffRepeatAsync(default, default, staffDto.PhoneNumber, staffDto.Email, default, false);
        if (staff is not null)
        {
            var updateStaffEvent = new UpdateStaffBeforeDomainEvent(default);
            await _staffDomainService.UpdateBeforeAsync(updateStaffEvent);
            staff.UpdateForLdap(staffDto.Name, staffDto.DisplayName, staffDto.PhoneNumber, staffDto.Email);
            await _staffRepository.UpdateAsync(staff);
            await _staffDomainService.UpdateAfterAsync(new(staff, default));
        }
        else
        {
            var addStaffDto = new AddStaffDto
            {
                Name = staffDto.Name,
                DisplayName = staffDto.DisplayName,
                Enabled = true,
                Email = staffDto.Email,
                PhoneNumber = staffDto.PhoneNumber,
                JobNumber = staffDto.JobNumber
            };
            await AddStaffAsync(addStaffDto);
        }
    }

    [EventHandler(1)]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        var staff = await _authDbContext.Set<Staff>()
                                            .Include(s => s.DepartmentStaffs)
                                            .Include(s => s.TeamStaffs)
                                            .FirstOrDefaultAsync(s => s.Id == command.Staff.Id);
        if (staff == null)
        {
            throw new UserFriendlyException("the current staff not found");
        }
        await _staffRepository.RemoveAsync(staff);
        await _staffDomainService.RemoveAsync(new(staff));
    }

    [EventHandler(1)]
    public async Task SyncAsync(SyncStaffCommand command)
    {
        var syncResults = new SyncStaffResultsDto();
        command.Result = syncResults;
        var syncStaffs = command.Staffs;
        //validation
        var validator = new SyncStaffValidator();
        foreach (var staff in syncStaffs)
        {
            var result = validator.Validate(staff);
            if (result.IsValid is false)
            {
                syncResults[staff.Index] = new()
                {
                    JobNumber = staff.JobNumber,
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }
        }
        //check duplicate
        CheckDuplicate(Staff => Staff.PhoneNumber);
        CheckDuplicate(Staff => Staff.JobNumber);
        CheckDuplicate(Staff => Staff.Email);
        CheckDuplicate(Staff => Staff.IdCard);
        if (syncResults.IsValid) return;

        //sync user
        var query = new AllUsersQuery();
        await _eventBus.PublishAsync(query);
        var allUsers = query.Result;
        var userRange = new List<User>();
        foreach (var syncStaff in syncStaffs)
        {
            try
            {
                var staff = await VerifyStaffRepeatAsync(default, syncStaff.JobNumber, syncStaff.PhoneNumber, syncStaff.Email, syncStaff.IdCard, false);
                if (staff is not null)
                {
                    var updateStaffEvent = new UpdateStaffBeforeDomainEvent(syncStaff.Position);
                    await _staffDomainService.UpdateBeforeAsync(updateStaffEvent);
                    staff.UpdateBasicInfo(syncStaff.Name, syncStaff.DisplayName, syncStaff.Gender, updateStaffEvent.PositionId, syncStaff.StaffType);
                    await _staffRepository.UpdateAsync(staff);
                    await _staffDomainService.UpdateAfterAsync(new(staff, default));
                }
                else
                {
                    var addStaffDto = new AddStaffDto
                    {
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Enabled = true,
                        Email = syncStaff.Email,
                        Password = syncStaff.Password,
                        PhoneNumber = syncStaff.PhoneNumber,
                        JobNumber = syncStaff.JobNumber,
                        IdCard = syncStaff.IdCard,
                        Position = syncStaff.Position,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                    };
                    await AddStaffAsync(addStaffDto);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex is UserFriendlyException ? ex.Message : "Unknown exception, please contact the administrator";
                syncResults[syncStaff.Index] = new()
                {
                    JobNumber = syncStaff.JobNumber,
                    Errors = new() { errorMsg }
                };
            }
        }

        void CheckDuplicate(Expression<Func<SyncStaffDto, string?>> selector)
        {
            var func = selector.Compile();
            if (syncStaffs.Where(staff => func(staff) is not null).IsDuplicate(func, out List<SyncStaffDto>? duplicate))
            {
                foreach (var staff in duplicate)
                {
                    syncResults[staff.Index] = new()
                    {
                        //Account = staff.Account,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }
    }

    private async Task<Staff?> VerifyStaffRepeatAsync(Guid? staffId, string? jobNumber, string? phoneNumber, string? email, string? idCard, bool throwException = true)
    {
        Expression<Func<Staff, bool>> condition = staff => false;
        if (!string.IsNullOrEmpty(jobNumber))
            condition = condition.Or(staff => staff.JobNumber == jobNumber);
        if (!string.IsNullOrEmpty(phoneNumber))
            condition = condition.Or(staff => staff.PhoneNumber == phoneNumber);
        if (!string.IsNullOrEmpty(email))
            condition = condition.Or(staff => staff.Email == email);
        if (!string.IsNullOrEmpty(idCard))
            condition = condition.Or(staff => staff.IdCard == idCard);
        if (staffId is not null)
        {
            Expression<Func<Staff, bool>> condition2 = staff => staff.Id != staffId;
            condition = condition2.And(condition);
        }

        var existStaff = await _staffRepository.FindAsync(condition);
        if (existStaff is not null)
        {
            if (throwException is false) return existStaff;
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == existStaff.PhoneNumber)
                throw new UserFriendlyException($"Staff with phone number {phoneNumber} already exists");
            if (string.IsNullOrEmpty(email) is false && email == existStaff.Email)
                throw new UserFriendlyException($"Staff with email {email} already exists");
            if (string.IsNullOrEmpty(idCard) is false && idCard == existStaff.IdCard)
                throw new UserFriendlyException($"Staff with idCard {idCard} already exists");
            if (string.IsNullOrEmpty(jobNumber) is false && jobNumber == existStaff.JobNumber)
                throw new UserFriendlyException($"Staff with jobNumber number {jobNumber} already exists");
        }
        return existStaff;
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler]
    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var exist = await _thirdPartyIdpRepository.GetCountAsync(thirdPartyIdp => thirdPartyIdp.Name == thirdPartyIdpDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ThirdPartyIdp with name {thirdPartyIdpDto.Name} already exists");

        var thirdPartyIdp = new ThirdPartyIdp(thirdPartyIdpDto.Name, thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, thirdPartyIdpDto.IdentificationType, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);

        await _thirdPartyIdpRepository.AddAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == thirdPartyIdpDto.Id);
        if (thirdPartyIdp is null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        thirdPartyIdp.Update(thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, thirdPartyIdpDto.IdentificationType, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);
        await _thirdPartyIdpRepository.UpdateAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task RemoveThirdPartyIdpAsync(RemoveThirdPartyIdpCommand command)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == command.ThirdPartyIdp.Id);
        if (thirdPartyIdp == null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        await _thirdPartyIdpRepository.RemoveAsync(thirdPartyIdp);
    }

    #endregion

    #region ThirdPartyUser

    [EventHandler(1)]
    public async Task AddThirdPartyUserAsync(AddThirdPartyUserCommand command)
    {
        var thirdPartyUserDto = command.ThirdPartyUser;
        await VerifyUserRepeatAsync(thirdPartyUserDto.ThirdPartyIdpId, thirdPartyUserDto.ThridPartyIdentity);
        await AddThirdPartyUserAsync(thirdPartyUserDto);
    }

    async Task AddThirdPartyUserAsync(AddThirdPartyUserDto dto)
    {
        var thirdPartyUseEvent = new AddThirdPartyUserBeforeDomainEvent(dto.User);
        await _thirdPartyUserDomainService.AddBeforeAsync(thirdPartyUseEvent);
        var thirdPartyUser = new ThirdPartyUser(dto.ThirdPartyIdpId, thirdPartyUseEvent.UserId, true, dto.ThridPartyIdentity, dto.ExtendedData);
        await _thirdPartyUserRepository.AddAsync(thirdPartyUser);
    }

    [EventHandler(1)]
    public async Task UpdateThirdPartyUserAsync(UpdateThirdPartyUserCommand command)
    {
        var thirdPartyUserDto = command.ThirdPartyUser;
        var thirdPartyUser = await _thirdPartyUserRepository.FindAsync(tpu => tpu.Id == thirdPartyUserDto.Id);
        if (thirdPartyUser is null)
            throw new UserFriendlyException("The current third party user does not exist");

        await VerifyUserRepeatAsync(thirdPartyUser.ThirdPartyIdpId, thirdPartyUserDto.ThridPartyIdentity);
        thirdPartyUser.Update(thirdPartyUserDto.Enabled, thirdPartyUserDto.ThridPartyIdentity, thirdPartyUserDto.ExtendedData);
        await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
    }

    [EventHandler(1)]
    public async Task UpsertThirdPartyUserForLdapAsync(UpsertThirdPartyUserForLdapCommand command)
    {
        var thirdPartyUser = await VerifyUserRepeatAsync(command.ThirdPartyIdpId, command.ThridPartyIdentity, false);
        if (thirdPartyUser is not null)
        {
            thirdPartyUser.Update(command.ThridPartyIdentity, command.ExtendedData);
            await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
            var resetUserPasswordCommand = new ResetUserPasswordCommand(new(thirdPartyUser.UserId, command.Password));
            await _eventBus.PublishAsync(resetUserPasswordCommand);
        }
        else
        {
            var addThirdPartyUserDto = new AddThirdPartyUserDto(command.ThirdPartyIdpId, true, command.ThridPartyIdentity, command.ExtendedData, command.Adapt<AddUserDto>());
            await AddThirdPartyUserAsync(addThirdPartyUserDto);
        }
        var upsertStaffDto = command.Adapt<UpsertStaffForLdapDto>();
        upsertStaffDto.UserId = thirdPartyUser?.UserId;
        var upsertStaffCommand = new UpsertStaffForLdapCommand(upsertStaffDto);
        await _eventBus.PublishAsync(upsertStaffCommand);
    }

    private async Task<ThirdPartyUser?> VerifyUserRepeatAsync(Guid thirdPartyIdpId, string thridPartyIdentity, bool throwException = true)
    {

        var thirdPartyUser = await _thirdPartyUserRepository.FindAsync(tpu => tpu.ThirdPartyIdpId == thirdPartyIdpId && tpu.ThridPartyIdentity == thridPartyIdentity);
        if (thirdPartyUser is not null)
        {
            if (throwException is false)
            {
                return thirdPartyUser;
            }
            throw new UserFriendlyException($"ThirdPartyUser with ThridPartyIdentity:{thridPartyIdentity} already exists");
        }
        return thirdPartyUser;
    }

    #endregion

    #region UserSystemData
    [EventHandler(1)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand command)
    {
        var data = command.UserSystemData;
        var item = await _userSystemBusinessDataRepository.FindAsync(userSystemBusinessData => userSystemBusinessData.UserId == data.UserId && userSystemBusinessData.SystemId == data.SystemId);
        if (item is null)
        {
            await _userSystemBusinessDataRepository.AddAsync(new UserSystemBusinessData(data.UserId, data.SystemId, data.Data));
        }
        else
        {
            item.Update(data.Data);
            await _userSystemBusinessDataRepository.UpdateAsync(item);
        }
    }
    #endregion
}
