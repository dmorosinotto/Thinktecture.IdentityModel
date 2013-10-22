Please see https://github.com/thinktecture/Thinktecture.IdentityModel/wiki/EmbeddedSts for more information on using EmbeddedSts.

Ensure the SAM and FAM are configured
--------------------------------------------------------
Before using EmbeddedSts you will need to ensure that the SAM and FAM are configured in web.config. You will need the configuration sections defined as such:

  <configSections>
    <section name="system.identityModel" type="System.IdentityModel.Configuration.SystemIdentityModelSection, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
    <section name="system.identityModel.services" type="System.IdentityModel.Services.Configuration.SystemIdentityModelServicesSection, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
  </configSections>

And the HTTP modules configured:

  <system.webServer>
    <modules>
      <add name="SessionAuthenticationModule" type="System.IdentityModel.Services.SessionAuthenticationModule, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" preCondition="managedHandler" />
      <add name="WSFederationAuthenticationModule" type="System.IdentityModel.Services.WSFederationAuthenticationModule, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" preCondition="managedHandler" />
    </modules>
  </system.webServer>

Enabling EmbeddedSts
--------------------------------------------------------
To then EmbeddedSts simply use the well-known issuer attribute on the <wsFederation> element:

  <system.identityModel.services>
    <federationConfiguration>
      <wsFederation passiveRedirectEnabled="true"
                    issuer="http://EmbeddedSts"
                    realm="http://localhost/MyApp/"
                    requireHttps="false"
      />
      <cookieHandler requireSsl="false" />
    </federationConfiguration>
  </system.identityModel.services>

Either http://EmbeddedSts or https://EmbeddedSts can be used.
