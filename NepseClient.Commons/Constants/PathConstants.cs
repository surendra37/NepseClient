using System;
using System.IO;

namespace NepseClient.Commons.Constants
{
    public static class PathConstants
    {
        public static Lazy<string> AppDataPath { get; } = new Lazy<string>(() =>
         {
             var commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
             var folderPath = Path.Combine(commonPath, "Surendra37", "NepseApp");
             if (!Directory.Exists(folderPath))
             {
                 Directory.CreateDirectory(folderPath);
             }
             return folderPath;
         });
    }
}
