namespace Shared.Infrastructure.Persistence.File
{
    public interface IFilePersistence
    {
        Task WriteLine(string filePath, bool appendData, string line);

        Task<List<string>> Read(string filePath);
    }
}
