using ECommerceAPI.Infrastructure.Operations;

namespace ECommerceAPI.Infrastructure.Services
{
    public class FileService
    {
        

        private async Task<string> FileRenameAsync(string path, string fileName, bool isFirst = true, int index = 0)
        {
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string newFileName;

            if (isFirst)
            {
                fileNameWithoutExtension = NameOperation.CharacterRegulatory(fileNameWithoutExtension);
                newFileName = $"{fileNameWithoutExtension}{extension}";
            }
            else
            {
                newFileName = $"{fileNameWithoutExtension}-{index}{extension}";
            }
            if (File.Exists($"{path}\\{newFileName}"))
            {
                index++;
                return await FileRenameAsync(path, fileName, false, index);
            }

            return newFileName;
        }

        

    }
}
