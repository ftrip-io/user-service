using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.user_service.Users.UseCases.UpdateUser
{
    public class UpdateUserRequest : IRequest<User>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
    }
}