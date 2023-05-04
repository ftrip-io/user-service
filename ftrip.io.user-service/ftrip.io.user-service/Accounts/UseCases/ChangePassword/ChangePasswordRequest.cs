using ftrip.io.user_service.Accounts.Domain;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.user_service.Accounts.UseCases.ChangePassword
{
    public class ChangePasswordRequest : IRequest<CredentialsAccount>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}