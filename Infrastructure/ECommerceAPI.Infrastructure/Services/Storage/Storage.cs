using ECommerceAPI.Infrastructure.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services.Storage
{
    public class Storage
    {
        protected delegate bool HasFile(string pathorContainerName, string fileName);
        protected async Task<string> FileRenameAsync(string pathorContainerName, string fileName, HasFile hasFileMethod,bool isFirst = true, int index = 0)
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
            //if (File.Exists($"{path}\\{newFileName}"))
            if (hasFileMethod(pathorContainerName, newFileName))
            {
                index++;
                return await FileRenameAsync(pathorContainerName, fileName, hasFileMethod, false, index);
            }

            return newFileName;
        }
    }
}
