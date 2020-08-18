using AuthReg.Data;
using AuthReg.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthReg.Models
{
    public class AuthenticationJWT : IAuthenticationJWT
    {

        //private readonly IDictionary<string, string> users = new Dictionary<string, string> { { "test1", "password1" } };
        private readonly string key;
        public AppDbContext db;

        public AuthenticationJWT (string key)
        {
            this.key = key;
        }

        public string Authenticate(string username, string password)
        {
         //if(!db.Users.Any(u=>u.UserName == username && u.Password ==password)){
         //       return null;
         //   }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
