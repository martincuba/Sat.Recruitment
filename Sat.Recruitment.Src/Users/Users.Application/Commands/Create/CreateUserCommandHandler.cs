using FluentValidation;
using MediatR;
using Users.Domain;
using Users.Domain.UserGif.Getter;

namespace Users.Application.Commands.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IGifCalculateGetter gifCalculateGetter;
        private readonly IUserRepository userRepository;
        private readonly IValidator<CreateUserCommand> validator;

        public CreateUserCommandHandler(
            IGifCalculateGetter gifCalculateGetter,
            IUserRepository userRepository,
            IValidator<CreateUserCommand> validator)
        {
            this.gifCalculateGetter = gifCalculateGetter;
            this.userRepository = userRepository;
            this.validator = validator;
        }

        public async Task<Unit> Handle(CreateUserCommand createUserCommand, CancellationToken cancellationToken)
        {
            await this.validator.ValidateAndThrowAsync(createUserCommand, cancellationToken);

            var newUser = CreateUserCommandMapper.Execute(createUserCommand);

            newUser.AddMoney(this.gifCalculateGetter
                .GetCalculator(newUser.UserType).Execute(newUser.Money));

            await this.userRepository.Save(newUser);

            return Unit.Value;
        }
    }
}
