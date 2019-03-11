using System;
using System.IO;
using System.Linq;
using WgetAnalogue.Interfaces;

namespace WgetAnalogue.Implementations
{
    /// <summary>
    /// The class that saves files to the specified directory.
    /// </summary>
    public class DataSaverToDirectory : IDataSaver
    {
        private readonly DirectoryInfo _directoryToSaveData;

        /// <summary>
        /// The ctor.
        /// </summary>
        /// <param name="directoryToSaveData">
        /// The directory files being saved to.
        /// </param>
        public DataSaverToDirectory(DirectoryInfo directoryToSaveData)
        {
            _directoryToSaveData = directoryToSaveData ?? throw new ArgumentNullException(nameof(directoryToSaveData));
        }

        /// <summary>
        /// Saves html documents.
        /// </summary>
        /// <param name="uri"> The uri of the html document.</param>
        /// <param name="title">The title of the html document.</param>
        /// <param name="fileStream"> The stream of the html document. </param>
        public void SaveHtmlFile(Uri uri, string title, Stream fileStream)
        {
            string pathToDirectory = CreateFullPath(uri);
            Directory.CreateDirectory(pathToDirectory);

            string newTitle = ValidateTitleAccordingCharsValidity(title);
            string fileFullPath = Path.Combine(pathToDirectory, ValidateTitleAccordingToLength(newTitle, pathToDirectory.Length));
            SaveToFile(fileFullPath, fileStream);
        }

        /// <summary>
        /// Saves source files.
        /// </summary>
        /// <param name="uri"> The uri of the source file.</param>
        /// <param name="fileStream"> The stream of the source file.</param>
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

        private string ValidateTitleAccordingCharsValidity(string title)
        {
            char[] invalidSymbols = Path.GetInvalidFileNameChars();

            return new string(title.Where(c => !invalidSymbols.Contains(c)).ToArray());
        }

        private string ValidateTitleAccordingToLength(string title, int directoryLength)
        {
            int pathLength = directoryLength + title.Length;
            var allowedPathLength = 259;

            return pathLength > allowedPathLength ? title.Substring(pathLength - allowedPathLength + 1) : title;
        }
    }
}