

public static class ImageHelper
{
    private static readonly string ImagesDirectory = Path.Combine(FileSystem.AppDataDirectory, "Images");

    public static async Task<string> SaveImageAsync(Stream imageStream, string uniqueId)
    {
        var folderPath = Path.Combine(FileSystem.AppDataDirectory, "ChildAvatars");
        Directory.CreateDirectory(folderPath);

        var imagePath = Path.Combine(folderPath, $"{uniqueId}.png");

        using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        return imagePath;
    }

    public static void DeleteImage(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
        }
    }
}
