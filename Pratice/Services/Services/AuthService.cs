using AutoMapper;
using BusinessLogicLayer.Services.IServices;
using DataAccess.Repositories.IRepositories;
using DataAccess.UoW;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model.Enum;
using Model.Helper;
using Model.Models.DTO;
using Model.Models.DTO.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;

        public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper, IConfiguration configuration, IMapper mapper)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var response = new ResponseDTO();

            try
            {
                // Get user by username
                var user = await _authRepository.GetByUserName(loginDTO.Username);
                if (user == null)
                {
                    response.Message = "Thông tin đăng nhập không hợp lệ";
                    return response;
                }

                // Verify password
                var isPasswordValid = VerifyPassword(loginDTO.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    response.Message = "Thông tin đăng nhập không hợp lệ";
                    return response;
                }

                // Check if email is verified (skip this check if needed for testing)
                if (!user.IsEmailVerified)
                {
                    response.Message = "Vui lòng xác minh email của bạn trước khi đăng nhập. Kiểm tra hộp thư đến để biết mã xác minh.";
                    return response;
                }

                // Check if account is active
                if (user.IsActive == AccountStatus.Banned)
                {
                    response.Message = "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ với quản trị viên để biết thêm chi tiết.";
                    return response;
                }

                // Generate tokens
                var token = _jwtHelper.GenerateJwtToken(user);
                var refreshToken = _jwtHelper.GenerateRefreshToken();

                var tokenExpiration = DateTime.Now.AddHours(1);
                var refreshTokenExpiration = DateTime.Now.AddDays(7);

                // Update user with tokens
                user.Token = token;
                user.TokenExpires = tokenExpiration;
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpires = refreshTokenExpiration;
                await _authRepository.UpdateAsync(user);

                // Return success response
                response.IsSucceed = true;
                response.Message = "Đăng nhập thành công!";
                response.Data = new { 
                    Token = token, 
                    RefreshToken = refreshToken,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role?.RoleName ?? user.RoleId.ToString()
                };

                return response;
            }
            catch (Exception ex)
            {
                response.Message = "Đã xảy ra lỗi trong quá trình đăng nhập: " + ex.Message;
                return response;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }

        private string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

