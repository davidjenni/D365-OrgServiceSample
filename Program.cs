// Copyright (c) 2020 David JENNI
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Tooling.Connector;

namespace OrgServiceSample
{
    public class Program
    {
        private static bool isAppUser;

        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length < 2 || (args.Length > 0 && string.CompareOrdinal(args[0], "-h") == 0))
            {
                Console.WriteLine($@"
Usage:
  {Process.GetCurrentProcess().ProcessName} <environmentUrl> <usernameOrAppId> [ <secret> ]

option: instead of passing the secret as the third parameter, set an environment variable 'PA_BT_ORG_SECRET' with that secret
");
                Environment.Exit(1);
            }
            var envUrl = new Uri(args.Length > 0 ? args[0] : "https://davidjenD365-1.crm.dynamics.com");
            var usernameOrAppId = args.Length > 1 ? args[1] : "2c6d7c95-ff20-4305-b87c-b97eb8277cf5";
            isAppUser = Guid.TryParse(usernameOrAppId, out var _);
            var secret = args.Length > 2 ? args[2] : Environment.GetEnvironmentVariable("PA_BT_ORG_SECRET");
            if (string.IsNullOrWhiteSpace(secret))
            {
                Console.WriteLine("Missing parameter 'secret' (or set env variable: 'PA_BT_ORG_SECRET'");
                Environment.Exit(1);
            }

            var app = new Program();
            var success = app.Connect(envUrl, usernameOrAppId, secret);
            Console.WriteLine($"Connected to '{envUrl}': {success}");
            Environment.Exit(success ? 0 : 1);
        }

        private bool Connect(Uri envUrl, string usernameOrAppId, string secret)
        {
            using (var crmClient = Create(envUrl, usernameOrAppId, secret))
            {
                if (!crmClient.IsReady)
                {
                    Console.WriteLine($"ERROR: Cannot connect to org '{envUrl}', error: {crmClient.LastCrmError}");
                    return false;
                }

                var whoAmI = new WhoAmIRequest();
                var response = (WhoAmIResponse)crmClient.ExecuteCrmOrganizationRequest(whoAmI);
                //var response = (WhoAmIResponse)crmClient.OrganizationWebProxyClient.Execute(whoAmI);
                if (response == null)
                {
                    Console.WriteLine($"ERROR: Cannot execute request from '{envUrl}', error: {crmClient.LastCrmError}");
                    return false;
                }
                Console.WriteLine($"Connected as user: {crmClient.OAuthUserId} (userId: {response.UserId}) to org: {crmClient.ConnectedOrgUniqueName} ({response.OrganizationId}, {crmClient.CrmConnectOrgUriActual})");
            }
            return true;
        }

        private static CrmServiceClient Create(Uri envUrl, string usernameOrAppId, string secret)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var promptBehavior = PromptBehavior.Never;
            // clientID and redirect url are values configured in Microsoft's tenant that are functional for 3rd parties sample code
            // please create specific clientID and redirect url in your PowerPlatform's AAD tenant
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUrl = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");

            var builder = new DbConnectionStringBuilder
            {
                { "Url", envUrl.AbsoluteUri },
                { "RedirectUri", redirectUrl.AbsoluteUri },
                { "LoginPrompt", promptBehavior.ToString() }
            };

            Console.WriteLine($"Connecting to env: {envUrl.AbsoluteUri}...");
            if (isAppUser)
            {
                Console.WriteLine($"... authN using appId & clientSecret - {usernameOrAppId}");
                builder.Add("AuthType", "ClientSecret");
                builder.Add("AppId", usernameOrAppId);
                builder.Add("ClientSecret", secret);
            }
            else
            {
                Console.WriteLine($"... authN using username & password - {usernameOrAppId}");
                builder.Add("AuthType", "OAuth");
                builder.Add("Username", usernameOrAppId);
                builder.Add("Password", secret);
                builder.Add("ClientId", clientId);
            }

            // if (isAppUser) { return new CrmServiceClient(envUrl, usernameOrAppId, secret, useUniqueInstance: true, string.Empty); }
            return new CrmServiceClient(builder.ConnectionString);
        }
    }
}
