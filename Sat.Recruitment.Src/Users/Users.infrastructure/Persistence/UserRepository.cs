using Shared.Domain;
using Shared.Domain.Exceptions;
using Shared.Infrastructure.Persistence.File;
using Users.Domain;

namespace Users.infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private const string SavedUsersFilePath = "/Files/Users.txt";
        private readonly IFilePersistence filePersistence;

        public UserRepository(IFilePersistence filePersistence)
        {
            this.filePersistence = filePersistence;
        }

        public async Task Save(User user)
        {
            try
            {
                await this.filePersistence.WriteLine(
                    GetFilePath(),
                    appendData: true,
                    UserFileMapper.ToLine(user));
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error ocurred trying to save a user", ex);
            }
        }

        public async Task<User?> Search(ISpecification<User> specification)
        {
            try
            {
                var stringUsers = await this.filePersistence.Read(GetFilePath());

                foreach (var userAsString in stringUsers)
                {
                    var savedUser = UserFileMapper.ToUser(userAsString);

                    if (specification.IsSatisfied(savedUser))
                    {
                        return savedUser;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error ocurred trying to get users", ex);
            }
        }

        private static string GetFilePath() => Directory.GetCurrentDirectory() + SavedUsersFilePath;
    }
}