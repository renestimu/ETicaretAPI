﻿using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandRequest : IRequest<UpdatePasswordCommandResponse>
    {
        // Add properties relevant to the request here
        public string UserId { get; set; }
        public string Password { get; set; }
        public string ResetToken { get; set; }
        public string PasswordConfirm { get; set; }

    }
}