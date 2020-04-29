# Dynamics365 / PowerPlatform simplest client app

Small Dynamics365 app to connect to environment instance and call WhoAmI, using CrmServiceClient from the CRM SDK.

Showcases how to initialize connection and handle errors.

## Prereqs:
- [VisualStudio 2019 Community](https://visualstudio.microsoft.com/downloads/)
- [.NET Framework 4.6.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net462); all of Dynamics365 still has a dependency on .NET 4.6.2 which unfortunately is not a default target for VS2019 (check if your VS already has it: open Properties of a loaded .csproj project, click dropdown box for target framework version)

## References

- [Build Windows client app using XRM tools](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/xrm-tooling/build-windows-client-applications-xrm-tools?view=dynamics-xrmtooling-ce-9)
- [CrmServiceClient](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient?view=dynamics-xrmtooling-ce-9)
- [WhoAmI requuest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.whoamirequest?view=dynamics-general-ce-9)
- [WhoAmI webAPI](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/whoami?view=dynamics-ce-odata-9)
