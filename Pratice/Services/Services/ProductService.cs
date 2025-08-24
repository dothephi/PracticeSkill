using AutoMapper;
using BusinessLogicLayer.Services.IServices;
using DataAccess.UoW;
using Model.Models;
using Model.Models.DTO;
using Model.Models.DTO.Product;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDTO> GetAllProductAsync()
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync();
                if (products == null || !products.Any())
                {
                    return new ResponseDTO
                    {
                        IsSucceed = false,
                        Message = "There is no any product"
                    };
                }
                var productDTOs = _mapper.Map<List<GetAllProductDTO>>(products);
                return new ResponseDTO
                {
                    IsSucceed = true,
                    Message = "Products retrieved successfully",
                    Data = productDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return new ResponseDTO
                {
                    IsSucceed = false,
                    Message = "Error retrieving products"
                };
            }
        }

        public async Task<ResponseDTO> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ResponseDTO
                    {
                        IsSucceed = false,
                        Message = "Product not found"
                    };
                }
                var productDTO = _mapper.Map<ProductDTO>(product);
                return new ResponseDTO
                {
                    IsSucceed = true,
                    Message = "Product retrieved successfully",
                    Data = productDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", productId);
                return new ResponseDTO
                {
                    IsSucceed = false,
                    Message = "Error retrieving product"
                };
            }
        }

        public async Task<ResponseDTO> CreateProductAsync(CreateProductDTO createProductDTO)
        {
            try
            {
                var product = _mapper.Map<Products>(createProductDTO);
                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.SaveChangeAsync();
                var productDTO = _mapper.Map<ProductDTO>(product);
                return new ResponseDTO
                {
                    IsSucceed = true,
                    Message = "Product created successfully",
                    Data = productDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return new ResponseDTO
                {
                    IsSucceed = false,
                    Message = "Error creating product"
                };
            }
        }

        public async Task<ResponseDTO> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO)
        {
            try
            {
                var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (existingProduct == null)
                {
                    return new ResponseDTO
                    {
                        IsSucceed = false,
                        Message = "Product not found"
                    };
                }
                _mapper.Map(updateProductDTO, existingProduct);
                await _unitOfWork.ProductRepository.UpdateAsync(existingProduct);
                await _unitOfWork.SaveChangeAsync();
                var productDTO = _mapper.Map<ProductDTO>(existingProduct);
                return new ResponseDTO
                {
                    IsSucceed = true,
                    Message = "Product updated successfully",
                    Data = productDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", productId);
                return new ResponseDTO
                {
                    IsSucceed = false,
                    Message = "Error updating product"
                };
            }
        }

        public async Task<ResponseDTO> DeleteProductAsync(int productId)
        {
            try
            {
                await _unitOfWork.ProductRepository.DeleteAsync(productId);
                await _unitOfWork.SaveChangeAsync();
                return new ResponseDTO
                {
                    IsSucceed = true,
                    Message = "Product deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", productId);
                return new ResponseDTO
                {
                    IsSucceed = false,
                    Message = "Error deleting product"
                };
            }
        }
    }
}
