﻿using AutoMapper;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Application.Validators;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Interfaces;
using Shared.Bases;

namespace ProductService.Application.Handlers
{
    public class ProductCommandHandler :
         IRequestHandler<CreateProductCommand, Response<string>>,
         IRequestHandler<UpdateProductCommand, Response<string>>,
         IRequestHandler<DeleteProductCommand, Response<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidateProductExists _validateProductExists;
        public readonly ResponseHandler _responseHandler;



        public ProductCommandHandler(IProductRepository productRepository, ResponseHandler responseHandler, IMapper mapper, IValidateProductExists validateProductExists)
        {
            _productRepository = productRepository;
            _validateProductExists = validateProductExists;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = _mapper.Map<Product>(request);
                if (request.Id != 0) await _validateProductExists.ValidateProductExistsAsync(request.Id);
                var addedProduct = await _productRepository.AddAsync(product);
                return _responseHandler.Created<string>("Product Created Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateProductExists.ValidateProductExistsAsync(request.Id);
                var product = _mapper.Map<Product>(request);
                var updatedProduct = await _productRepository.UpdateAsync(product);
                return _responseHandler.Success<string>("Product Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }
        public async Task<Response<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateProductExists.ValidateProductExistsAsync(request.Id);
                await _productRepository.DeleteByIdAsync(request.Id);
                return _responseHandler.Success<string>("Product Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }
    }
}