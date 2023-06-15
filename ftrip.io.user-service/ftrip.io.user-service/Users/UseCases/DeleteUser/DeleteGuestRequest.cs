using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;

namespace ftrip.io.user_service.Users.UseCases.DeleteUser
{
    public class DeleteGuestRequest : IRequest<User>
    {
        public Guid GuestId { get; set; }
    }
}