# uSplit - A/B Testing package for Umbraco

This repository contains the source code of uSplit, an Umbraco plugin that removes all technical barriers to A/B testing.

The information in this readme is inteded for developers wishing to discuss and contribute to the project.

#### uSpit user documentation

For supplementary information visit the [uSplit product page](http://endzonesoftware.com/usplit-ab-testing-for-umbraco/), where you can also learn about various support packages that are available. We have also made an [installation guide](http://endzonesoftware.com/installing-usplit/) available, and a tutorial on how to [run your first experiment](http://endzonesoftware.com/running-ab-experiments-like-pro-usplit/). These topics were also covered and demoed on a recent [uHangout episode](https://www.youtube.com/watch?v=WQysVNLyQM8).  

## Distribution

uSplit is available via [NuGet](https://www.nuget.org/packages/Endzone.uSplit) (the preffered way of installation) and is also listed on the [Umbraco package repository](https://our.umbraco.org/projects/website-utilities/usplit/).

### NuGet

Currently there are two parallel versions of this plugin available. `1.0.x` for Umbraco <= 7.5 and `1.6.x` for Umbraco 7.6+. Despite the minor version increase these two versions contain the same functionality. Pick one that matches your Umbraco version.

Our CI server builds every commit and all successful builds are automatically packaged and published on [NuGet](https://www.nuget.org/packages/Endzone.uSplit).

```
PM> Install-Package Endzone.uSplit
```

#### NuGet gotchas (versoin 1.0.x; Umbraco <= 7.5 only)

The [Google Analytics Expriments C# Libraries](https://github.com/google/google-api-dotnet-client) have a dependcy on a strongly-signed log4net library version 1.2.13. Up until 7.5 Umbraco was built against and distributed an older unsigned library - version 1.2.11. Due to the signing mismatch [both versions need to be provided to the application](http://i386.com/2015/02/umbraco-and-log4net-using-two-different-versions-of-a-dll-in-asp-net/).

To do so, uSplit places the library Umbraco looks for into `bin/log4net/1.2.11.0/log4net.dll`. The following has to be added to Web.config (under `//configuration/runtime/assemblyBinding`) to make the library discoverable:

```
<dependentAssembly>
  <assemblyIdentity name="log4net" publicKeyToken="null" />
  <codeBase version="1.2.11.0" href="bin/log4net/1.2.11.0/log4net.dll" />
</dependentAssembly>     
```

### Umbraco package repository

We also manually release new versions on the [Umbraco package repository](https://our.umbraco.org/projects/website-utilities/usplit/). Since this is a manual process these updates are available later than via NuGet, and not all versions might be released here.

#### Umbraco package repository gotchas (versoin 1.0.x; Umbraco <= 7.5 only)

This method of installation does not update any NuGet packages. uSplit however requires `Newtonsoft.Json` version 7 or above.

`Newtonsoft.Json` version 7 is therefore packaged with uSplit `1.0.x` and will get copied over into the target bin folder when you install the pacakge. If you by any chance used a newer version than the one we distribute make sure to place it back!

Target site needs to update the assembly binding redirect to point to version 7 (unless it is using a newer version):

```
<dependentAssembly>
  <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
  <bindingRedirect oldVersion="0.0.0.0-7.0.0.0" newVersion="7.0.0.0" />
</dependentAssembly>
```

uSplit version `1.6.x` targetting Umbraco `7.6.0` does not distribute this library as version 10 is pulled down as a dependency on `Umbraco.Core`.

## Contributions

Pull requests are more than welcome! Fixed a bug or improved the editorial experience? Share your code with the rest of the community! We do not have official coding standards, just try to keep the code consistent.

Feel free to use the issue tracker to report any problems you encounter. Unless you have a support contract with us, we do not provide any SLAs.

### Dev Resources

- An example [Google API ASP.NET MVC integration with OAuth 2.0](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-aspnet-mvc)