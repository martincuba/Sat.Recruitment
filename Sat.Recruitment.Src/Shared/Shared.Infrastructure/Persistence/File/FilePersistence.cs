namespace Shared.Infrastructure.Persistence.File
{
    public class FilePersistence : IFilePersistence
    {
        public async Task<List<string>> Read(string filePath)
        {
            FileStream fileStream = new(filePath, FileMode.Open);
            StreamReader reader = new(fileStream);

            List<string> lines = new();

            while (reader.Peek() >= 0)
            {
                var line = await reader.ReadLineAsync();
                lines.Add(line);
            }
            reader.Close();

            return lines;
        }

        public async Task WriteLine(string filePath, bool appendData, string line)
        {
            using (StreamWriter usersFile = new(filePath, append: appendData))
            {
                await usersFile.WriteLineAsync(line);
            }
        }
    }
}
