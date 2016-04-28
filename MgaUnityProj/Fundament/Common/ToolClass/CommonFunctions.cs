using System.IO;

public class CommonFunctions
{
    public static void CreateFolder(string sFullFileName)
    {
        sFullFileName = sFullFileName.Replace("\\", "/");
        string[] dirs = sFullFileName.Split('/');
        string startPath = dirs[0];
        for (int i = 1; i < dirs.Length - 1; ++i)
        {
            startPath += ("/" + dirs[i]);
            if (!Directory.Exists(startPath))
            {
                Directory.CreateDirectory(startPath);
            }
        }
    }
}
