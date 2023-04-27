using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.user_service.Users.Domain
{
    public class User : Entity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public UserType Type { get; set; }
    }
}