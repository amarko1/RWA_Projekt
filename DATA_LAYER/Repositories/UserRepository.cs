using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
using DATA_LAYER.JWTModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.Repositories
{
    public interface IUserRepository
    {
        BLUser Add(UserRegisterRequest request);
        void ValidateEmail(ValidateEmailRequest request);
        Tokens JwtTokens(JwtTokensRequest request);
        void ChangePassword(ChangePasswordRequest request);
        bool Authenticate(string username, string password);
    }

    public class UserRepository : IUserRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration, IMapper mapper, RwaMoviesContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public BLUser Add(UserRegisterRequest request)
        {
            var normalizedUsername = request.Username.ToLower().Trim();
            if (_dbContext.Users.Any(x => x.Username.Equals(normalizedUsername)))
                throw new InvalidOperationException("Username already exists");

            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); 
            string b64Salt = Convert.ToBase64String(salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);

            var newUser = new BLUser
            {
                Username = request.Username,
                Email = request.Email,
                IsConfirmed = false,
                SecurityToken = b64SecToken
            };

            var dbUser = _mapper.Map<User>(newUser);
            _dbContext.Users.Add(dbUser);
            _dbContext.SaveChanges();

            newUser.Id = dbUser.Id; 
            return newUser;
        }

        public void ValidateEmail(ValidateEmailRequest request)
        {
            var target = _dbContext.Users.FirstOrDefault(x =>
                x.Username == request.Username && x.SecurityToken == request.B64SecToken);

            if (target == null)
                throw new InvalidOperationException("Authentication failed");

            target.IsConfirmed = true;
            _dbContext.SaveChanges();
        }

        public bool Authenticate(string username, string password)
        {
            var target = _dbContext.Users.Single(x => x.Username == username);

            if (!target.IsConfirmed)
                return false;

            byte[] salt = Convert.FromBase64String(target.PwdSalt);
            byte[] hash = Convert.FromBase64String(target.PwdHash);

            byte[] calcHash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

            return hash.SequenceEqual(calcHash);
        }

        public Tokens JwtTokens(JwtTokensRequest request)
        {
            var isAuthenticated = Authenticate(request.Username, request.Password);

            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");

            var jwtKey = _configuration["JWT:Key"];
            var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                new System.Security.Claims.Claim(ClaimTypes.Name, request.Username),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, request.Username)
                }),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return new Tokens
            {
                Token = serializedToken
            };
        }

        public void ChangePassword(ChangePasswordRequest request)
        {
            var isAuthenticated = Authenticate(request.Username, request.OldPassword);

            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");

            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); 
            string b64Salt = Convert.ToBase64String(salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.NewPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            var target = _dbContext.Users.Single(x => x.Username == request.Username);
            target.PwdSalt = b64Salt;
            target.PwdHash = b64Hash;
            _dbContext.SaveChanges();
        }
    }
}
