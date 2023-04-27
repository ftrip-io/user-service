using ftrip.io.user_service.Accounts.Domain;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.user_service.Accounts.UseCases.CreateAccount
{
    public class CreateAccountRequest : IRequest<CredentialsAccount>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}