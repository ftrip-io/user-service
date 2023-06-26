using ftrip.io.user_service.Accounts.Domain;
using MediatR;
using System;

namespace ftrip.io.user_service.Accounts.UseCases.DeleteAccount
{
    public class DeleteAccountRequest : IRequest<CredentialsAccount>
    {
        public Guid AccountId { get; set; }
    }
}