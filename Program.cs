using System;
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var password = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("PA_BT_ORG_PASSWORD");
            var username = "davidjen@davidjenD365.onmicrosoft.com";
            var envUrl = new Uri("https://davidjenD365-1.crm.dynamics.com");

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
            var promptBehavior = PromptBehavior.Never;
            var appId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUrl = "app://58145B91-0C36-4500-8554-080854F2AC97";
            var connectionString = $"AuthType=OAuth;Username={username};Password={password};Url={envUrl};AppId={appId};RedirectUri={redirectUrl};LoginPrompt={promptBehavior}";
            return new CrmServiceClient(connectionString);
        }
    }
}
