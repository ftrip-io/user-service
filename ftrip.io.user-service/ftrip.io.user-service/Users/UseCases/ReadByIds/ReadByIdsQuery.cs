using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.user_service.Users.UseCases.ReadByIds
{
    public class ReadByIdsQuery : IRequest<IEnumerable<User>>
    {
        public Guid[] Ids { get; set; }
    }
}