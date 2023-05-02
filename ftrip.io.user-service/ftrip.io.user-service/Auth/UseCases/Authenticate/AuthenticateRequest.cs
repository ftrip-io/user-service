using MediatR;

namespace ftrip.io.user_service.Auth.UseCases.Authenticate
{
    public class AuthenticateRequest : IRequest<AuthenticatedUser>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}