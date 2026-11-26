namespace asdfasdf;

public static class Utils
{
    public static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}