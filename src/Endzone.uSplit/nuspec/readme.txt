Hello!

Thank you for installing uSplit, the A/B testing plugin for Umbraco.

Some manual changes to Web.config are required before the site can be accessed. Please add the following lines to the runtime.assemblyBinding section.

<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
	<assemblyIdentity name="log4net" publicKeyToken="null" />
	<codeBase version="1.2.11.0" href="bin/log4net/1.2.11.0/log4net.dll" />
  </dependentAssembly>
</assemblyBinding>

Depending on the Newtonsoft.Json package you had installed, you might need to manually update the assemblyBinding information for this library as well.
Check the binding and if it points to version 6, change it as follows:

<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
    <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-7.0.0.0" newVersion="7.0.0.0" />
  </dependentAssembly>
</assemblyBinding>

Once these are in place the site should start. To finish uSplit installation you still need to set the 5 new appSettings values (added automatically as part of this NuGet package):

<add key="uSplit:googleClientId" value="" />
<add key="uSplit:googleClientSecret" value="" />
<add key="uSplit:accountId" value="" />
<add key="uSplit:webPropertyId" value="" />
<add key="uSplit:profileId" value="" />

To find out where to obtain these, and for further help with the installation please visit: http://www.happyporch.com/installing-usplit/

