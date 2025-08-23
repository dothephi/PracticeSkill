using Model.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.IServices
{
    public interface IProductService
    {
        Task<ResponseDTO> CreateProductAsync(CreateAdminDTO createProductDTO);
        Task<ResponseDTO> GetProductByIdAsync(int adminId);
        Task<ResponseDTO> UpdateProductAsync(int adminId, UpdateAdminDTO updateAdminDTO);
        Task<ResponseDTO> GetAllProductAsync();
        Task<ResponseDTO> DeleteProductAsync();
    }
}
