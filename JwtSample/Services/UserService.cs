using JwtSample.Models;
using JwtSample.Services.Interface;

namespace JwtSample.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "Mike", Password = "Test1234=" },
            new User { Id = 1, Username = "Admin", Password = "Test1234=" }
        };

        public User? Authenticate(string username, string password)
        {
            User? user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);
            return user;
        }

        public bool Checkrefresh(string access_Token, string refresh_Token)
        {
            return (_users.FirstOrDefault(uu => uu.RefreshToken == refresh_Token && uu.AccessToken == access_Token) == null) ;
        }
    }
}
