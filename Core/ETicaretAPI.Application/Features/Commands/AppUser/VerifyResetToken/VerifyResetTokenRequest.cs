using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.VerifyResetToken
{
    public class VerifyResetTokenRequest :IRequest<VerifyResetTokenResponse>
    {
        public string ResetToken { get; set; }
        public string UserId { get; set; }
    }
}