using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;

namespace NotForgottenTwo.Services
{
    public class TkenManager
    {
        //size is small so can be used in mobile devices
        //JSON Web Token


        //Token encoded using Json

        public void ProduceToken()
        {
            //var token = new JwtSecurityToken(
            //    issuer:"SpringMoth",
            //    audience: "TokenReceiver",
            //    claims:GetClaims(),
            //    signingCredentials: GetKey(),
            //    ValidFrom:DateTime.UtcNow,
            //    ValidTo: DateTime.UtcNow.AddDays(30));

        }

        public void ConsumeToken()
        {

        }

    }
}