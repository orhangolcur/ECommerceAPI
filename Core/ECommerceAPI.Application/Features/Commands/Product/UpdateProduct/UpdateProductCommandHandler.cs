﻿using ECommerceAPI.Application.Repositories;
using MediatR;
using P = ECommerceAPI.Domain.Entities;


namespace ECommerceAPI.Application.Features.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;

        public UpdateProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
        {
            P.Product product = await _productReadRepository.GetByIdAsync(request.Id);
            product.Price = request.Price;
            product.Name = request.Name;
            product.Stock = request.Stock;
            await _productWriteRepository.SaveAsync();
            return new();
        }
    }
}
