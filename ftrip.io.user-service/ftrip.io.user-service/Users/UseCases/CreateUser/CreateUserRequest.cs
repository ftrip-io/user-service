using ftrip.io.framework.Mapping;
using ftrip.io.user_service.Accounts.UseCases.CreateAccount;
using ftrip.io.user_service.Users.Domain;
using MediatR;

namespace ftrip.io.user_service.Users.UseCases.CreateUser
{
    [Mappable(Destination = typeof(User))]
    public class CreateUserRequest : IRequest<User>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public UserType Type { get; set; }

        public CreateAccountRequest Account { get; set; }
    }
}