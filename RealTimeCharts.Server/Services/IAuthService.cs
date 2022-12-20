using RealTimeCharts.Server.Models;

namespace RealTimeCharts.Server.Services
{
    public interface IAuthService
    {
        public IConfiguration config { get; }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        string CreateToken(User user);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
