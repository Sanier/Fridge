using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Fridge.DAL.Interfaces;
using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Domain.Response;
using Fridge.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fridge.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private const int ExpectedHashSize = 64;
        private const int ExpectedSaltSize = 128;

        private readonly IBaseRepositories<UserEntity> _userRepository;
        private readonly JwtModel _jwtSettings;

        public UserService(IBaseRepositories<UserEntity> userRepository, IOptions<JwtModel> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<IBaseResponse<UserModel>> CheckIn(long userId)
        {
            try
            {
                var user = _userRepository.Get()
                .FirstOrDefaultAsync(x => x.UserId == userId);

                return new BaseResponse<UserModel>()
                {
                    Description = $"The task has been create",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserModel>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<UserEntity>> Create(UserModel user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                    throw new ArgumentNullException(nameof(user));

                if (await _userRepository.Get().AnyAsync(x => x.Email == user.Email))
                    throw new ArgumentException("Username \"" + user.Email + "\" is already taken");

                byte[] passwordHash, passwordSalt;

                CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

                var newUser = new UserEntity
                {
                    Email = user.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };

                await _userRepository.Create(newUser);
                await _userRepository.Update(newUser);

                return new BaseResponse<UserEntity>()
                {
                    Description = $"The task has been create",
                    StatusCode = HttpStatusCode.Accepted
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserEntity>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<UserModel>>> GetUsers()
        {
            try
            {
                var list = await _userRepository.Get()
                    .Select(l => new UserModel
                    {
                        Email= l.Email
                    })
                    .ToListAsync();

                if (list is null)
                    throw new ArgumentNullException();

                return new BaseResponse<IEnumerable<UserModel>>()
                {
                    Data = list,
                    Description = $"The task has been create",
                    StatusCode = HttpStatusCode.Accepted
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<UserModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<string>> Authenticate(UserModel user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                    throw new ArgumentNullException(nameof(user));

                var authUser = await _userRepository.Get()
                .Include(o => o.Products)
                .SingleOrDefaultAsync(x => x.Email == user.Email);

                if (authUser == null)
                    throw new ArgumentNullException(nameof(authUser));

                if (!VerifyPasswordHash(user.Password, authUser.PasswordHash, authUser.PasswordSalt))
                {
                    throw new ArgumentException("Все говно и тебя нет");
                }
                else
                {
                    var token = GenerateToken(user);
                    return new BaseResponse<string>()
                    {
                        Data = token,
                        Description = $"The task has been create",
                        StatusCode = HttpStatusCode.Accepted
                    };
                }
                    
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        #region Private method

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) 
                throw new ArgumentNullException("password");

            if (string.IsNullOrWhiteSpace(password)) 
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));
            if (storedHash.Length != ExpectedHashSize)
                throw new ArgumentException($"Invalid length of password hash ({ExpectedHashSize} bytes expected).", nameof(storedHash));
            if (storedSalt.Length != ExpectedSaltSize)
                throw new ArgumentException($"Invalid length of password salt ({ExpectedSaltSize} bytes expected).", nameof(storedSalt));

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private string GenerateToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
    }
}
