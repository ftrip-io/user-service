using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;

namespace ftrip.io.user_service.Users.UseCases.ReadById
{
    public class ReadByIdQuery : IRequest<User>
    {
        public Guid Id { get; set; }
    }
}