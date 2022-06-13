﻿using MediatR;
using Users.Domain;
using Users.Domain.Specifications;
using Users.Domain.UserGif.Getter;
using ApplicationException = Shared.Domain.Exceptions.ApplicationException;

namespace Users.Application.Commands.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IGifCalculateGetter gifCalculateGetter;
        private readonly IUserRepository userRepository;

        public CreateUserCommandHandler(
            IGifCalculateGetter gifCalculateGetter,
            IUserRepository userRepository)
        {
            this.gifCalculateGetter = gifCalculateGetter;
            this.userRepository = userRepository;
        }

        public async Task<Unit> Handle(CreateUserCommand createUserCommand, CancellationToken cancellationToken)
        {
            var newUser = CreateUserCommandMapper.Execute(createUserCommand);

            var existingUser = await this.userRepository.Search(new ExistingUserSpecification(
                newUser.Name,
                newUser.Email,
                newUser.Address,
                newUser.Phone));

            if (existingUser is not null)
            {
                throw new ApplicationException("The user is duplicated");
            }

            newUser.AddMoney(this.gifCalculateGetter
                .GetCalculator(newUser.UserType).Execute(newUser.Money));

            await this.userRepository.Save(newUser);

            return Unit.Value;
        }
    }
}
