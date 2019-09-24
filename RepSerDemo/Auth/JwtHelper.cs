using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepSerDemo.Auth
{
    public class JwtHelper
    {
        public static IConfiguration Configuration { get; set; }

        public string GetJwtStr()
        {

            var iss = Configuration.GetSection("JWT:Issuer").Value;
            var aud = Configuration.GetSection("JWT:Audience").Value;
            var sec = Configuration.GetSection("JWT:Secret").Value;


            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Iss,iss),
                 new Claim(JwtRegisteredClaimNames.Aud,aud),
                  new Claim(JwtRegisteredClaimNames.Exp,
                  $"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sec));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(issuer: iss, claims:claims, signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

    }
}
