using JwtSample.Infra;
using JwtSample.Models;
using JwtSample.Services.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions _jwtOption;
        private readonly IUserService _userService;
        public AuthController(JwtOptions jwtoption, IUserService userService)
        {
            _jwtOption= jwtoption;
            _userService=userService;
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
            User? u = _userService.Authenticate(lm.NickName, lm.Password);
            if (u==null)
            {
                return new BadRequestObjectResult(lm);
            }
             

            string leTokenArenvoyer = JwtManager.GenerateToken(_jwtOption,  u);


            return Ok(new JwtResponse {   Access_Token=leTokenArenvoyer, Refresh_Token=u.RefreshToken  });

        }

        [HttpPost("refresh")]
        public IActionResult Refresh(JwtResponse tokenResponse)
        {
            List<Claim> mesClaims = null;
            //Récupération des claims du user (idéalement dans une db car normalement l'user n'existe plus si j'ai besoin 
            // de faire un refresh
            if (User !=null)
            {
                  mesClaims = User.Claims.ToList();
                 
            }
            else
            {
                //rechercher les rôles dans
            }

            //Vérification si le token correspond
            if (_userService.Checkrefresh(tokenResponse.Access_Token, tokenResponse.Refresh_Token))
            {
                string newAccessToken = JwtManager.GenerateAccessTokenFromRefreshToken(tokenResponse.Refresh_Token, _jwtOption, mesClaims);

                JwtResponse response = new JwtResponse
                {
                    Access_Token = newAccessToken,
                    Refresh_Token = JwtManager.GenerateRefreshToken() // Return the same refresh token
                };
                //On doit ici resauvegarder en db les infos (ici ce n'est pas fait car pas de db mais une liste statique)

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
             
           
        }
    }
}
