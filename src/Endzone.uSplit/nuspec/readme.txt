Hello!

Thank you for installing uSplit, the A/B testing plugin for Umbraco.

To finalize the installation please add the following lines to your web.config runtime.assemblyBinding section.

<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
	<assemblyIdentity name="log4net" publicKeyToken="null" />
	<codeBase version="1.2.11.0" href="bin/log4net/1.2.11.0/log4net.dll" />
  </dependentAssembly>
</assemblyBinding>

For further help with the installation visit: http://www.happyporch.com/installing-usplit/

