﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstractions.Storage.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage.Azure
{
    public class AzureStorage :Storage, IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;

        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }

        public async Task DeleteAsync(string container, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
            BlobClient blobClient= _blobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        public List<string> GetFiles(string container)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
            return _blobContainerClient.GetBlobs().Select(x=>x.Name).ToList();
        }

        public bool HasFile(string container, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
            return _blobContainerClient.GetBlobs().Any(b => b.Name == fileName);
        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string container, IFormFileCollection files)
        {
            _blobContainerClient =  _blobServiceClient.GetBlobContainerClient(container);
            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
            List<(string fileName, string path)> datas = new();
            foreach (IFormFile file in files)
            {
               string fileNewName =  await FileRenameAsync(container, file.FileName, HasFile);
                BlobClient blobClient= _blobContainerClient.GetBlobClient(fileNewName);
                await blobClient.UploadAsync(file.OpenReadStream());
                datas.Add((fileNewName, $"{container}/{fileNewName}"));
            }
            return datas;
        }
    }
}
