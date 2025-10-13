using IndiaTrails.API.Models.Domain;

namespace IndiaTrails.API.Repositories.Contracts;

public interface IAuthRepository
{
    Task<User?> RegisterAsync(User user, string password);
    Task<User?> LoginAsync(string email, string password);
    Task<bool> UserExistsAsync(string email);
    string GenerateJwtToken(User user);
}
