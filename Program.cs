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
        [STAThread]
        public static void  Main(string[] args)
        {
            if (args.Length < 2 || (args.Length > 0 && string.CompareOrdinal(args[0], "-h") == 0))
            {
                Console.WriteLine($@"
Usage:
  {Process.GetCurrentProcess().ProcessName} <environmentUrl> <username> [ <password> ]

option: instead of passing the password as the third parameter, set an environment variable 'PA_BT_ORG_PASSWORD' with that password
");
                Environment.Exit(1);
            }
            var envUrl = new Uri(args.Length > 0 ? args[0] : "https://davidjenD365-1.crm.dynamics.com");
            var username = args.Length > 1 ? args[1] : "davidjen@davidjenD365.onmicrosoft.com";
            var password = args.Length > 2 ? args[2] : Environment.GetEnvironmentVariable("PA_BT_ORG_PASSWORD");
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Missing parameter 'password' (or set env variable: 'PA_BT_ORG_PASSWORD'");
                Environment.Exit(1);
            }

            var app = new Program();
            Console.WriteLine($"Connected to '{envUrl}': {app.Connect(envUrl, username, password)}");

            Console.Read();
        }

        private bool Connect(Uri envUrl, string username, string password)
        {
            using (var crmClient = Create(envUrl, username, password))
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

        private static CrmServiceClient Create(Uri envUrl, string username, string password)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var promptBehavior = PromptBehavior.Never;
            var appId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUrl = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");

            var builder = new DbConnectionStringBuilder
            {
                { "AuthType", "OAuth" },
                { "Url", envUrl.AbsoluteUri },
                { "RedirectUri", redirectUrl.AbsoluteUri },
                { "LoginPrompt", promptBehavior.ToString() }
            };

            builder.Add("Username", username);
            builder.Add("Password", password);
            builder.Add("AppId", appId);

            return new CrmServiceClient(builder.ConnectionString);
        }
    }
}
