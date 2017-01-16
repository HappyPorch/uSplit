          ____        _ _ _   
    _   _/ ___| _ __ | (_) |_ 
   | | | \___ \| '_ \| | | __|
   | |_| |___) | |_) | | | |_ 
    \__,_|____/| .__/|_|_|\__|
               |_|           

             Hello!

  Thank you for installing uSplit,
the A/B testing plugin for Umbraco.


Installation - ACTION REQUIRED!
-------------------------------

Installting this package likely broke your site! Some manual changes
to Web.config are required before the site can be accessed. 

Please add the following lines to the runtime.assemblyBinding section:

  <dependentAssembly>
	<assemblyIdentity name="log4net" publicKeyToken="null" />
	<codeBase version="1.2.11.0" href="bin/log4net/1.2.11.0/log4net.dll" />
  </dependentAssembly>

Once these are in place the site should start.

For details visit: http://www.endzonesoftware.com/installing-usplit/


Configuration
-------------

To start using uSplit you need to set 5 new appSettings values which have
been added automatically as part of this NuGet package installation:

<add key="uSplit:googleClientId" value="" />
<add key="uSplit:googleClientSecret" value="" />
<add key="uSplit:accountId" value="" />
<add key="uSplit:webPropertyId" value="" />
<add key="uSplit:profileId" value="" />

To find out how to obtain these, and for further help with the installation
please visit: http://www.endzonesoftware.com/installing-usplit/


Notes
-----

uSplit product page: http://endzonesoftware.com/usplit-ab-testing-for-umbraco/
uSplit uHangout talk and demo: https://www.youtube.com/watch?v=WQysVNLyQM8
uSplit source code: https://github.com/EndzoneSoftware/uSplit