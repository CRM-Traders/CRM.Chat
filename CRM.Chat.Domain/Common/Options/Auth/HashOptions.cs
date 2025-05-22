using System.Security.Cryptography;

namespace CRM.Chat.Domain.Common.Options.Auth;

public static class HashOptions
{
    public const int keySize = 64;
    public const int iterations = 350000;
    public static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
}