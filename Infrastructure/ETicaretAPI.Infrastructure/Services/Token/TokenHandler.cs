﻿using ETicaretAPI.Application.Abstractions.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken( int second)
        {
            Application.DTOs.Token token = new();
            //simetriğini alıyoruz
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
            //Şifrelenmiş kimliği oluşturuyoruz
            SigningCredentials signingCredentials= new(securityKey,SecurityAlgorithms.HmacSha256);
            token.Expiration=DateTime.UtcNow.AddMinutes(second);
            JwtSecurityToken tokenSecurityToken = new(
                audience: _configuration["Tokken:Audience"], 
                issuer: _configuration["Token:Issuer"],
                expires:token.Expiration,
                notBefore:DateTime.UtcNow,
                signingCredentials:signingCredentials
                );
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            token.AccessToken=handler.WriteToken(tokenSecurityToken);
            return token;
        }
    }
}
