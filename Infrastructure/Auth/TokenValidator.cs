using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;

public static class TokenValidator
{
    public static ClaimsPrincipal? ValidateToken(string token, string tenantId, string audience)
    {
        try
        {
            var authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{authority}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever()
            );

            var openIdConfig = configManager.GetConfigurationAsync(CancellationToken.None).Result;

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = audience,
                ValidIssuers = new[]
                {
                $"https://login.microsoftonline.com/{tenantId}/v2.0",
                $"https://sts.windows.net/{tenantId}/"
            },
                IssuerSigningKeys = openIdConfig.SigningKeys,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out var _);

            Console.WriteLine("Token successfully validated!");
            return principal;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation exception: {ex.Message}");
            return null;
        }
    }

}
