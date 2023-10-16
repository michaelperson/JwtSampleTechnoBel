using JwtSample.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions _jwtOption;
        public AuthController(JwtOptions jwtoption)
        {
            _jwtOption= jwtoption;
        }


        [HttpPost]
        public IActionResult Login(LoginModel lm)
        {
            if(lm==null)
            {
                return new NotFoundResult();
            }
            if(!ModelState.IsValid) 
            { 
                return new BadRequestObjectResult(ModelState); 
            } 
            
            if((lm.NickName != "Admin" && lm.NickName != "Mike") || lm.Password!="Test1234=")
            {
                return new BadRequestObjectResult(lm);
            }

            //Générer le token et le renvoyer
            //1- string key vers byte key
            byte[] skey = Encoding.UTF8.GetBytes(_jwtOption.SigningKey);
            SymmetricSecurityKey laCle = new SymmetricSecurityKey(skey);

            //2- Quleque claims
            Claim infoNom = new Claim(ClaimTypes.Name, "Mike");
            Claim Rols = new Claim(ClaimTypes.Role, lm.NickName=="Mike"?"Boulet":"Admin");

            List<Claim> mesClaims = new List<Claim>();
            mesClaims.Add(infoNom);
            mesClaims.Add(Rols);

            JwtSecurityToken Token = new JwtSecurityToken(

                 issuer: _jwtOption.Issuer,
                 audience: _jwtOption.Audience,
                 claims: mesClaims,
                 expires: DateTime.Now.AddSeconds(_jwtOption.Expiration),
                 signingCredentials: new SigningCredentials(laCle, SecurityAlgorithms.HmacSha256)

             );
            string leTokenArenvoyer = new JwtSecurityTokenHandler().WriteToken(Token);
            return Ok(new { expiration= _jwtOption.Expiration, access_token=leTokenArenvoyer });

        }
    }
}
