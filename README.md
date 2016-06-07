# uSplit

A/B Testing plugin for Umbraco

# Setup

## Site

1. Install the plugin
  - Ignore binary errors
1. Update Newtonsoft.Json.dll to version 7
  - via NuGet or just by adding the following assemlby redirect (dll is in Package)
```
<dependentAssembly>
  <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
  <bindingRedirect oldVersion="0.0.0.0-7.0.0.0" newVersion="7.0.0.0" />
</dependentAssembly>
```
1. Copy both versions of log4net to bin
  - see instructions bellow
1. Add the app settings to web.config
  - provide the values you obtain from Google

### App settings

- uSplit:googleClientId
- uSplit:googleClientSecret
- uSplit:accountId
- uSplit:webPropertyId
- uSplit:profileId

### Two versions of log4net

The [Google Analytics Expriments C# Libraries](https://github.com/google/google-api-dotnet-client) have a dependcy on log4net version 1.2.13 while Umbraco still depends on 1.2.11.0. [Both versions need to be provided to the application](http://i386.com/2015/02/umbraco-and-log4net-using-two-different-versions-of-a-dll-in-asp-net/). To do so, add this to Web.config (under `//configuration/runtime/assemblyBinding`):

```
<dependentAssembly>
  <assemblyIdentity name="log4net" publicKeyToken="669e0ddf0bb1aa2a" />
  <codeBase version="1.2.13.0" href="bin/log4net/1.2.13.0/log4net.dll" />
</dependentAssembly>
<dependentAssembly>
  <assemblyIdentity name="log4net" publicKeyToken="null" />
  <codeBase version="1.2.11.0" href="bin/log4net/1.2.11.0/log4net.dll" />
</dependentAssembly>     
```

Both dlls need to be copied to the paths indicated in the above.

## Google Analytics

A [Google API](https://console.developers.google.com/apis) [project](https://console.developers.google.com/project) client credentials [are required](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-aspnet-mvc). They are used to [authenticate calls to Google](https://developers.google.com/identity/protocols/OAuth2WebServer) when calling it's API (which go against a quota for each project). These should be managed and provided by the owner of the site. 

1. [Register a project](https://console.developers.google.com/project)
1. [Enable the Analytics API for the project](https://console.developers.google.com/apis/enabled)
1. Customize the [OAuth consent screen](https://console.developers.google.com/apis/credentials/consent) 
1. [Generate OAuth Client ID credentials](https://console.developers.google.com/apis/credentials/oauthclient)
  - Application type is "Web application"
  - Set `Authorized JavaScript origins`
    - don't forget to list all your live, staging, and dev domains
    - for local testing it is recommended to create a fake domain with a hosts file entry
  - For the `Authorized redirect URIs` field you will need to set absolute URLs with the following path: `/umbraco/backoffice/usplit/GoogleCallback/IndexAsync`


### Obtaining Google Analytics Experiments client credentials

#Resources

- An example [Google API ASP.NET MVC integration with OAuth 2.0](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-aspnet-mvc)