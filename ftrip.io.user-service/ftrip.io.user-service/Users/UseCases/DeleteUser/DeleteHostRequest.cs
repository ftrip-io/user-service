using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;

namespace ftrip.io.user_service.Users.UseCases.DeleteUser
{
    public class DeleteHostRequest : IRequest<User>
    {
        public Guid HostId { get; set; }
    }
}