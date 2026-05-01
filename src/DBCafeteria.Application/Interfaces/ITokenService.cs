using DBCafeteria.Domain.Entities;

namespace DBCafeteria.Application.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Cliente cliente);
    string CreateRefreshToken();
}
