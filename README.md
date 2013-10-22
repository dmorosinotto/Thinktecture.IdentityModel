# Thinktecture IdentityModel #

This is the successor to the very popular [Thinktecture.IdentityModel.45](https://github.com/thinktecture/Thinktecture.IdentityModel.45) repository. The old project has reached a certain size where it made more sense to break up the different features areas in separate assemblies and projects.

The new IdentityModel consists of the following parts:

### Core ###
- Base64Url encoding
- Epoch Date Time conversion
- Random number generation
- Time-constant string comparison
- Certificate Store Access
- Useful constants when dealing with algorithms, date time formats, tokens and protocols
- Anoynmous claims principal
- Authentication instant claim
- Claims-based authorization
- ClaimsPrincipal factory
- Extension Methods for XML, security token conversion, X.509 certificates

### Extensions and Middleware for OWIN/Katana ###
- Claims transformation
- Token format support
- Support for retrieving tokens from headers or query strings

### HTTP Security / OAuth2 Client Library
- Portable library (.NET 4.5, Windows 8, Windows Phone 8)
- OAuth2 client library
- Helpers for HttpClient for dealing with Basic Authentication and token headers

### Embedded STS ###
- Easy to use embeddable, zero-config STS for ASP.NET


### WCF ###
- WS-Trust bindings for UserName, Windows, Issued Tokens and X.509 certificates
- Helpers for dealing with RSTRs and entropy
- WS-Trust Client

### SWT ###
- WIF integration for Simple Web Tokens