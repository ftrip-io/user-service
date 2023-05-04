using ftrip.io.user_service.Accounts.UseCases.ChangePassword;
using ftrip.io.user_service.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Accounts
{
    [ApiController]
    [Route("api/users")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [UserSpecific]
        [HttpPut("{userId}/account/password")]
        public async Task<IActionResult> ChangePassword(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            request.UserId = userId;

            return Ok(await _mediator.Send(request, cancellationToken));
        }
    }
}