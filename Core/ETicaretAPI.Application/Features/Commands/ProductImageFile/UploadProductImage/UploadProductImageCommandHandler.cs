﻿using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
    internal class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IStorageService _storageService;

        public UploadProductImageCommandHandler(IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IStorageService storageService)
        {
            _productReadRepository = productReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
        }

        public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", request.Files);
           Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id);


            await _productImageFileWriteRepository.AddRangeAsync(result.Select(d => new Domain.Entities.ProductImageFile()
            {
                FileName = d.fileName,
                Path = d.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Domain.Entities.Product>() { product }
            }).ToList());
            await _productImageFileWriteRepository.SaveAsync();

            return new();

            
        }
    }
}
