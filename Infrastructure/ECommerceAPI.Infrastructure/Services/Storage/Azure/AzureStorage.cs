using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ECommerceAPI.Application.Abstractions.Storage.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services.Storage.Azure
{
    public class AzureStorage : Storage,IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient; // ilgili azure storage hesabına erişim sağlar
        BlobContainerClient _blobContainerClient; // blob container ile etkileşimde bulunmamızı sağlar

        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }

        public async Task DeleteAsync(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName); 
            await blobClient.DeleteAsync();

        }

        public List<string> GetFiles(string containerName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Select(blobItem => blobItem.Name).ToList(); // container içindeki tüm dosyaların adlarını döner
        }

        public bool HasFile(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Any(blobItem => blobItem.Name == fileName);
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string containerName, IFormFileCollection files) //parametre gönderirken dikkat etmeliyiz, pathOrContainerName olarak containerName gönderiyoruz. Oradaki pathOrContainerName'i değiştirmemiz gerekiyor.
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName); // containerName ile blobContainerClient'i alıyoruz
            await _blobContainerClient.CreateIfNotExistsAsync(); // container varsa hata vermez, yoksa oluşturur
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer); // container'a erişim izni verir

            List<(string fileName, string pathOrContainerName)> datas = new();
            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(containerName, file.Name, HasFile);

                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNewName); // blobClient, dosyayı azure storage'a yüklemek için kullanılır
                await blobClient.UploadAsync(file.OpenReadStream()); // dosyayı yükler
                datas.Add((fileNewName, $"{containerName}/{fileNewName}")); // pathOrContainerName olarak containerName ve dosya adını ekliyoruz
            }
            return datas;
        }
    }
}
