﻿using AspNetCoreHero.Results;
using ELMCOM.Application.DTOs.Identity;
using System.Threading.Tasks;

namespace ELMCOM.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress);

        Task<Result<string>> RegisterAsync(RegisterRequest request, string origin);

        Task<Result<string>> ConfirmEmailAsync(string userId, string code);

        Task ForgotPassword(ForgotPasswordRequest model, string origin);

        Task<Result<string>> ResetPassword(ResetPasswordRequest model);
    }
}