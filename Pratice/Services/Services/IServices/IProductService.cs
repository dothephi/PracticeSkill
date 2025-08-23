using Model.Models.DTO;
using Model.Models.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.IServices
{
    public interface IProductService
    {
        Task<ResponseDTO> CreateProductAsync(CreateProductDTO createProductDTO);
        Task<ResponseDTO> GetProductByIdAsync(int productId);
        Task<ResponseDTO> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO);
        Task<ResponseDTO> GetAllProductAsync();
        Task<ResponseDTO> DeleteProductAsync(int productId);
    }
}
