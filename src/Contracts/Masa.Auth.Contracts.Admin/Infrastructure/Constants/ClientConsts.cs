﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class ClientConsts
{
    public static readonly string[] MandatoryResource = new string[] { "openid", "profile" };

    public static List<SelectItemDto<string>> GetSecretTypes()
    {
        var secretTypes = (new List<string>
            {
                "SharedSecret",
                "X509Thumbprint",
                "X509Name",
                "X509CertificateBase64",
                "JWK"
            }).Select(a => new SelectItemDto<string> { Value = a, Text = a }).ToList();

        return secretTypes;
    }

    public static List<SelectItemDto<string>> GetStandardClaims()
    {
        //http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        var standardClaims = (new List<string>
            {
                "name",
                "given_name",
                "family_name",
                "middle_name",
                "nickname",
                "preferred_username",
                "profile",
                "picture",
                "website",
                "gender",
                "birthdate",
                "zoneinfo",
                "locale",
                "address",
                "updated_at"
            }).Select(a => new SelectItemDto<string> { Value = a, Text = a }).ToList();

        return standardClaims;
    }

    public static List<SelectItemDto<string>> SigningAlgorithms()
    {
        var signingAlgorithms = (new List<string>
            {
                "RS256",
                "RS384",
                "RS512",
                "PS256",
                "PS384",
                "PS512",
                "ES256",
                "ES384",
                "ES512"
            }).Select(a => new SelectItemDto<string> { Value = a, Text = a }).ToList();

        return signingAlgorithms;
    }

    public static List<SelectItemDto<string>> GetProtocolTypes()
    {
        var protocolTypes = new List<SelectItemDto<string>> { new SelectItemDto<string>("oidc", "OpenID Connect") };

        return protocolTypes;
    }

    public static List<SelectItemDto<int>> GetAccessTokenTypes()
    {
        var accessTokenTypes = EnumUtil.GetList<AccessTokenType>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
        return accessTokenTypes;
    }

    public static List<SelectItemDto<int>> GetTokenExpirations()
    {
        var tokenExpirations = EnumUtil.GetList<TokenExpiration>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
        return tokenExpirations;
    }

    public static List<SelectItemDto<int>> GetTokenUsage()
    {
        var tokenUsage = EnumUtil.GetList<TokenUsage>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
        return tokenUsage;
    }

    public const string AUTH_CLIENT_DEFAULT_URI = "https://cdn.masastack.com/stack/auth/ico/auth-client-default.svg";
}
