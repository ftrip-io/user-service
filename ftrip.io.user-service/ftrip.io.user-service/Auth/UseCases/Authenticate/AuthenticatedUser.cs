using ftrip.io.user_service.Users.Domain;

namespace ftrip.io.user_service.Auth.UseCases.Authenticate
{
    public class AuthenticatedUser
    {
        public User User { get; set; }
        public string Token { get; set; }
    }
}