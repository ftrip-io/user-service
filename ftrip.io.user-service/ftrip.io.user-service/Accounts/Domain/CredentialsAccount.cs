using ftrip.io.framework.Domain;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.user_service.Accounts.Domain
{
    public class CredentialsAccount : Entity<Guid>
    {
        public string Username { get; set; }

        [JsonIgnore]
        public string HashedPassword { get; set; }

        [JsonIgnore]
        public string Salt { get; set; }

        public Guid UserId { get; set; }
    }
}