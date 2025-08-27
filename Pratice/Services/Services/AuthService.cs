using BusinessLogicLayer.Services.IServices;
using DataAccess.Repositories.IRepositories;
using DataAccess.UoW;
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
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper, ILogger<AuthService> logger, IUnitOfWork unitOfWork)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var response = new ResponseDTO();

            try
            {
                _logger.LogInformation($"Attempting login for username/email: {loginDTO.Username}");

                // Tìm theo Username
                var user = await _authRepository.GetByUserName(loginDTO.Username);

                if (user == null)
                {
                    _logger.LogWarning($"User not found: {loginDTO.Username}");
                    response.Message = "Thông tin đăng nhập không hợp lệ";
                    return response;
                }

                _logger.LogInformation($"User found: {user.Username}, verifying password...");

                // Kiểm tra password
                var isPasswordValid = VerifyPassword(loginDTO.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    _logger.LogWarning($"Invalid password for user: {loginDTO.Username}");
                    response.Message = "Thông tin đăng nhập không hợp lệ";
                    return response;
                }

                // Kiểm tra email
                if (!user.IsEmailVerified)
                {
                    _logger.LogWarning($"Email not verified for user: {loginDTO.Username}");
                    response.Message = "Vui lòng xác minh email của bạn trước khi đăng nhập.";
                    return response;
                }

                // Kiểm tra trạng thái
                if (user.IsActive == AccountStatus.Banned)
                {
                    _logger.LogWarning($"Account banned for user: {loginDTO.Username}");
                    response.Message = "Tài khoản đã bị khóa. Liên hệ admin để biết thêm.";
                    return response;
                }

                // Sinh token
                var token = _jwtHelper.GenerateJwtToken(user);
                var refreshToken = _jwtHelper.GenerateRefreshToken();

                user.Token = token;
                user.TokenExpires = DateTime.UtcNow.AddHours(1);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);

                await _authRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangeAsync();

                response.IsSucceed = true;
                response.Message = "Đăng nhập thành công!";
                response.Data = new { Token = token, RefreshToken = refreshToken };

                _logger.LogInformation($"Login successful for user: {loginDTO.Username}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user: {loginDTO.Username}");
                response.Message = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại sau.";
                return response;
            }
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty");

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(hashedPassword))
            {
                _logger.LogWarning("Password or hash is null/empty");
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }
    }
}

