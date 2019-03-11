using System;
using System.IO;

namespace WgetAnalogue.Interfaces
{
    /// <summary>
    /// Determines how to save files.
    /// </summary>
    public interface IDataSaver
    {
        /// <summary>
        /// Saves html documents.
        /// </summary>
        /// <param name="uri"> The uri of the html document.</param>
        /// <param name="title">The title of the html document.</param>
        /// <param name="fileStream"> The stream of the html document. </param>
        void SaveHtmlFile(Uri uri, string title, Stream fileStream);

        /// <summary>
        /// Saves source files.
        /// </summary>
        /// <param name="uri"> The uri of the source file.</param>
        /// <param name="fileStream"> The stream of the source file.</param>
        void SaveSourceFile(Uri uri, Stream fileStream);
    }
}