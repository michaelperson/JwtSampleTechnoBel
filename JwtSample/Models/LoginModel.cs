using System.ComponentModel.DataAnnotations;

namespace JwtSample.Models
{
    public class LoginModel
    {
        private string _nickName;
        private string _password;

        [Required]
        public string NickName
        {
            get
            {
                return _nickName;
            }

            set
            {
                _nickName = value;
            }
        }

        [Required]
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }
    }
}
