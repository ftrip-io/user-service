using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.UpdateUser
{
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, User>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IStringManager _stringManager;

        public UpdateUserRequestHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IStringManager stringManager)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _stringManager = stringManager;
        }

        public async Task<User> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingUser = await ReadOrThrow(request.Id, cancellationToken);
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.City = request.City;

            await _userRepository.Update(existingUser, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return existingUser;
        }

        private async Task<User> ReadOrThrow(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Read(userId, cancellationToken);
            if (user == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", userId));
            }

            return user;
        }
    }
}