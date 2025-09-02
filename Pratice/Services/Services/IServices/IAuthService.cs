using Model.Models.DTO;
using Model.Models.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<ResponseDTO> LoginAsync(LoginDTO loginDTO);
    }
}
