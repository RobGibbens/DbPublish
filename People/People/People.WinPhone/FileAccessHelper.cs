using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace People.WinPhone
{
    public class FileAccessHelper
    {
        public static string GetLocalFilePath(string filename)
        {
            string path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            string dbPath = Path.Combine(path, filename);

            CopyDatabaseIfNotExists(dbPath);

            return dbPath;
        }

        public static void CopyDatabaseIfNotExists(string dbPath)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

            if (!iso.FileExists(dbPath))
            {
                // Create a stream for the file in the installation folder.
                using (Stream input = Application.GetResourceStream(new Uri("people.db3", UriKind.Relative)).Stream)
                {
                    // Create a stream for the new file in the local folder.
                    using (IsolatedStorageFileStream output = iso.CreateFile(dbPath))
                    {
                        // Initialize the buffer.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the file from the installation folder to the local folder. 
                        while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            output.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
    }
}
