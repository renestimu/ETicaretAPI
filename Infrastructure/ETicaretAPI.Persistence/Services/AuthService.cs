﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        readonly IUserService _userService;
        readonly IMailService _mailService;
        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, SignInManager<AppUser> signInManager, IUserService userService, IMailService mailService)
        {
            _httpClient = httpClient.CreateClient();
            _configuration = configuration;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
            _mailService = mailService;
        }

        async Task<Token> CreateUserExternalAsync(AppUser appUser, string email, string name, UserLoginInfo info, int accessTokenLifeTime)
        {

            bool result = appUser != null;

            if (appUser == null)
            {
                appUser = await _userManager.FindByEmailAsync(email);
                if (appUser == null)
                {
                    appUser = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name,
                    };
                    var identity = await _userManager.CreateAsync(appUser);
                    result = identity.Succeeded;

                }
                result = appUser != null;

            }
            if (result)
            {
                await _userManager.AddLoginAsync(appUser, info);


                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, appUser);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, appUser, token.Expiration, 900);
                return token;
            }



            throw new Exception("Invalid external authentication");
        }



        public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:Client_ID"]}" +
                $"&client_secret={_configuration["ExternalLoginSettings:Facebook:Client_Screet"]}&grant_type=client_credentials");


            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);
            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);
            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");
                FacebookUserInfoResponse? infoResponse = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);


                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");
                Domain.Entities.Identity.AppUser appUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                return await CreateUserExternalAsync(appUser, infoResponse?.Email, infoResponse.Name, info, accessTokenLifeTime);

            }

            throw new Exception("Invalid external authentication");
        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_ID"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");
            Domain.Entities.Identity.AppUser appUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            return await CreateUserExternalAsync(appUser, payload?.Email, payload?.Name, info, accessTokenLifeTime);
        }


        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null)
                throw new NotFoundUserException();


            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (signInResult.Succeeded)
            {
                Token token = _tokenHandler.CreateAccessToken(900, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 300);
                return token;
            }
            throw new AuthenticationErrorException();
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            Domain.Entities.Identity.AppUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
            if (user != null && user?.ReRefreshTokenEndDate > DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(900, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 300);
                return token;
            }
            else
                throw new NotFoundUserException();

        }

        public async Task PasswordResetAsync(string email)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                //byte[] tokenBytes = Encoding.UTF8.GetBytes(resetToken);
                //resetToken = WebEncoders.Base64UrlEncode(tokenBytes);
               resetToken=resetToken.UrlEncode();

                await _mailService.SendPasswordResetEmailAsync(email, appUser.Id, resetToken);
            }
        }

        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                //byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
                //resetToken = Encoding.UTF8.GetString(tokenBytes);
                resetToken=resetToken.UrlDecode();
             return  await  _userManager.VerifyUserTokenAsync(appUser,_userManager.Options.Tokens.PasswordResetTokenProvider,"ResetPassword", resetToken);
            }
            return false;
        }
    }
}
