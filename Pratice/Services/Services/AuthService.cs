using BusinessLogicLayer.Services.IServices;
using DataAccess.Repositories.IRepositories;
using Model.Enum;
using Model.Helper;
using Model.Models.DTO;
using Model.Models.DTO.Auth;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<ResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var response = new ResponseDTO();
            
            try 
            {
                _logger.LogInformation($"Attempting login for username: {loginDTO.Username}");

                var user = await _authRepository.GetByUserName(loginDTO.Username);
                if (user == null)
                {
                    _logger.LogWarning($"User not found: {loginDTO.Username}");
                    response.Message = "Thông tin đăng nhập không hợp lệ";
                    return response;
                }

                _logger.LogInformation($"User found, verifying password for user: {loginDTO.Username}");
                _logger.LogDebug($"Stored hash format: {user.PasswordHash?.Substring(0, Math.Min(user.PasswordHash?.Length ?? 0, 10))}...");

                _logger.LogInformation($"Comparing entered password: {loginDTO.Password} with hash: {user.PasswordHash}");
                var isPasswordValid = VerifyPassword(loginDTO.Password, user.PasswordHash);
                _logger.LogInformation($"Password valid: {isPasswordValid}");

                if (!isPasswordValid)
                {
                    _logger.LogWarning($"Invalid password for user: {loginDTO.Username}");
                    response.Message = "Thông tin đăng nhập không hợp lệ";
                    return response;
                }

                if (!user.IsEmailVerified)
                {
                    _logger.LogWarning($"Email not verified for user: {loginDTO.Username}");
                    response.Message = "Vui lòng xác minh email của bạn trước khi đăng nhập. Kiểm tra hộp thư đến để biết mã xác minh.";
                    return response;
                }

                if (user.IsActive == AccountStatus.Banned)
                {
                    _logger.LogWarning($"Account banned for user: {loginDTO.Username}");
                    response.Message = "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ với quản trị viên để biết thêm chi tiết.";
                    return response;
                }

                _logger.LogInformation($"Generating tokens for user: {loginDTO.Username}");
                
                try
                {
                    var token = _jwtHelper.GenerateJwtToken(user);
                    var refreshToken = _jwtHelper.GenerateRefreshToken();

                    var tokenExpiration = DateTime.UtcNow.AddHours(1);
                    var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

                    user.Token = token;
                    user.TokenExpires = tokenExpiration;
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpires = refreshTokenExpiration;

                    await _authRepository.UpdateAsync(user);
                    _logger.LogInformation($"Tokens updated for user: {loginDTO.Username}");

                    response.IsSucceed = true;
                    response.Message = "Đăng nhập thành công!";
                    response.Data = new { Token = token, RefreshToken = refreshToken };

                    _logger.LogInformation($"Login successful for user: {loginDTO.Username}");
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating or saving tokens");
                    throw;
                }
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

            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(hashedPassword))
            {
                _logger.LogWarning("Password or hash is null or empty");
                return false;
            }

            try
            {
                _logger.LogDebug($"Attempting to verify password. Hash format: {hashedPassword.Substring(0, Math.Min(hashedPassword.Length, 10))}...");
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

