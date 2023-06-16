using ftrip.io.user_service.Attributes;
using ftrip.io.user_service.Users.UseCases.CreateUser;
using ftrip.io.user_service.Users.UseCases.DeleteUser;
using ftrip.io.user_service.Users.UseCases.ReadById;
using ftrip.io.user_service.Users.UseCases.ReadByIds;
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

        [HttpGet]
        public async Task<IActionResult> ReadByIds([FromQuery] Guid[] userIds, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ReadByIdsQuery() { Ids = userIds }, cancellationToken));
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

        [Authorize]
        [UserSpecific]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteUserRequest() { UserId = userId }, cancellationToken));
        }
    }
}