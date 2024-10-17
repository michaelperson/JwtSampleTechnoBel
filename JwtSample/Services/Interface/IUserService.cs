using JwtSample.Models;

namespace JwtSample.Services.Interface
{
    public interface IUserService
    {
        User? Authenticate(string username, string password);
        bool Checkrefresh(string access_Token, string refresh_Token);
    }
}
