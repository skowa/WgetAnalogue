using System;
using System.IO;
using System.Linq;
using WgetAnalogue.Interfaces;

namespace WgetAnalogue.Implementations
{
    public class DataSaverToDirectory : IDataSaver
    {
        private readonly DirectoryInfo _directoryToSaveData;

        public DataSaverToDirectory(DirectoryInfo directoryToSaveData)
        {
            _directoryToSaveData = directoryToSaveData ?? throw new ArgumentNullException(nameof(directoryToSaveData));
        }

        public void SaveHtmlFile(Uri uri, string title, Stream fileStream)
        {
            string pathToDirectory = CreateFullPath(uri);
            Directory.CreateDirectory(pathToDirectory);

            string fileFullPath = Path.Combine(pathToDirectory, ValidateTitle(title));
            SaveToFile(fileFullPath, fileStream);
        }

        public void SaveSourceFile(Uri uri, Stream fileStream)
        {
            string fullPath = CreateFullPath(uri);
            string directoryPath = Path.GetDirectoryName(fullPath);
            Directory.CreateDirectory(directoryPath);

            if (Directory.Exists(fullPath))
            {
                fullPath = Path.Combine(fullPath, Path.GetRandomFileName());
            }

            SaveToFile(fullPath, fileStream);
        }

        private void SaveToFile(string path, Stream fileStream)
        {
            using (fileStream)
            using (var stream = File.Create(path))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.CopyTo(stream);
            }
        }

        private string CreateFullPath(Uri uri) => _directoryToSaveData + uri.LocalPath.Replace('/', '\\');

        private string ValidateTitle(string title)
        {
            char[] invalidSymbols = Path.GetInvalidFileNameChars();
            return new string(title.Where(c => !invalidSymbols.Contains(c)).ToArray());
        }
    }
}