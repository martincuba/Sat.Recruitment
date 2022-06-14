using FluentValidation;
using Users.Domain;
using Users.Domain.Specifications;

namespace Users.Application.Commands.Create
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserRepository userRepository;

        public CreateUserCommandValidator(IUserRepository userRepository)
        {
            this.userRepository = userRepository; 
            this.ClassLevelCascadeMode = CascadeMode.Stop;

            this.RuleFor(x => x.Name)
                .NotEmpty();

            this.RuleFor(x => x.Address)
                .NotEmpty();
            
            this.RuleFor(x => x.Money)
                .GreaterThanOrEqualTo(decimal.Zero);

            RuleFor(x => x).MustAsync(async (x, cancellation) =>
            {
                bool isDuplicated = await IsDuplicate(x);

                return !isDuplicated;
            });
        }

        private async Task<bool> IsDuplicate(CreateUserCommand command)
        {
            var existingUser = await this.userRepository.Search(new ExistingUserSpecification(
                command.Name,
                command.Email,
                command.Address,
                command.Phone));

            return existingUser is not null;
        }
    }
}
