using System.Security.Cryptography;
using DBCafeteria.Application.Interfaces;

namespace DBCafeteria.Infrastructure.Security;

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public byte[] Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return [.. salt, .. key];
    }

    public bool Verify(string password, byte[] hash)
    {
        if (hash.Length != SaltSize + KeySize)
            return false;

        var salt = hash[..SaltSize];
        var storedKey = hash[SaltSize..];
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return CryptographicOperations.FixedTimeEquals(key, storedKey);
    }
}
