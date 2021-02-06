using NepseClient.Commons.Constants;

namespace NepseClient.Commons.Utils
{
    public static class PathUtils
    {
        public static string ReplacePathHolders(string path)
        {
            return path.Replace("%AppData%", PathConstants.AppDataPath.Value);
        }
    }
}
