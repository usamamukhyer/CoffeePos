namespace DBCafeteria.Application.Interfaces;

public interface IPasswordHasher
{
    byte[] Hash(string password);
    bool Verify(string password, byte[] hash);
}
