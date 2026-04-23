using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace IdentityProvider.WebApi.Helpers;

public static class CertificateHelper
{
    public static ECDsaSecurityKey GetECDsaSigningKey(ReadOnlySpan<char> key)
    {
        var algorithm = ECDsa.Create();
        algorithm.ImportFromPem(key);

        return new ECDsaSecurityKey(algorithm);
    }
}
