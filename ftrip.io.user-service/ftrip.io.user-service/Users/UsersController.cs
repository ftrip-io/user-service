using ftrip.io.user_service.Attributes;
using ftrip.io.user_service.Users.UseCases.CreateUser;
using ftrip.io.user_service.Users.UseCases.ReadById;
using ftrip.io.user_service.Users.UseCases.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("{userId}")]
        public async Task<IActionResult> ReadById(Guid userId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ReadByIdQuery() { Id = userId }, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(request, cancellationToken));
        }

        [Authorize]
        [UserSpecific]
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            request.Id = userId;
            return Ok(await _mediator.Send(request, cancellationToken));
        }
    }
}