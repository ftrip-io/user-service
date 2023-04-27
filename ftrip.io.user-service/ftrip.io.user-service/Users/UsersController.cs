using ftrip.io.user_service.Users.UseCases.CreateUser;
using ftrip.io.user_service.Users.UseCases.ReadById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ReadById(Guid id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ReadByIdQuery() { Id = id }, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(request, cancellationToken));
        }
    }
}