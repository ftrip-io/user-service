using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.user_service.contracts.Users.Events
{
    public class UserDeletedEvent : Event<string>
    {
        public string UserId { get; set; }
        public string UserType { get; set; }

        public UserDeletedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}