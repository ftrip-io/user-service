using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.user_service.contracts.Users.Events
{
    public class UserCreatedEvent : Event<string>
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public UserCreatedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}