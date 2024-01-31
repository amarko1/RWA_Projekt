using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.Repositories
{

    public interface IUserRepo
    {
        IEnumerable<BLUser> GetAll();
        BLUser CreateUser(string username, string firstName, string lastName, string email, string password, int countryId);
        void ConfirmEmail(string email, string securityToken);
        BLUser GetConfirmedUser(string username, string password);
        void ChangePassword(string username, string newPassword);
        BLUser Get(string username);
    }

    public class UserRepo : IUserRepo
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepo(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLUser> GetAll()
        {
            var dbUsers = _dbContext.Users;

            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dbUsers);

            return blUsers;
        }

        public BLUser CreateUser(string username, string firstName, string lastName, string email, string password, int countryId)
        {
            (var salt, var b64Salt) = GenerateSalt();
            var b64Hash = CreateHash(password, salt);
            var b64SecToken = GenerateSecurityToken();

            var dbUser = new User()
            {
                CreatedAt = DateTime.UtcNow,
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PwdHash = b64Hash,
                PwdSalt = b64Salt,
                SecurityToken = b64SecToken,
                CountryOfResidenceId = countryId
            };
            _dbContext.Users.Add(dbUser);

            _dbContext.SaveChanges();

            var blUser = _mapper.Map<BLUser>(dbUser);
            return blUser;
        }

        public void ConfirmEmail(string email, string securityToken)
        {
            var userToConfirm = _dbContext.Users.FirstOrDefault(x =>
                x.Email == email &&
                x.SecurityToken == securityToken);

            userToConfirm.IsConfirmed = true;

            _dbContext.SaveChanges();
        }

        public BLUser Get(string username)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (dbUser == null)
            {
                return null;
            }

            var blUser = _mapper.Map<BLUser>(dbUser);

            return blUser;
        }

        public BLUser GetConfirmedUser(string username, string password)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.IsConfirmed == false);

            var salt = Convert.FromBase64String(dbUser.PwdSalt);
            var b64Hash = CreateHash(password, salt);

            if (dbUser.PwdHash != b64Hash)
                throw new InvalidOperationException("Wrong username or password");

            var blUser = _mapper.Map<BLUser>(dbUser);

            return blUser;
        }

        public void ChangePassword(string username, string newPassword)
        {
            var userToChangePassword = _dbContext.Users.FirstOrDefault(x =>
                x.Username == username &&
                !x.DeletedAt.HasValue);

            (var salt, var b64Salt) = GenerateSalt();

            var b64Hash = CreateHash(newPassword, salt);

            userToChangePassword.PwdHash = b64Hash;
            userToChangePassword.PwdSalt = b64Salt;

            _dbContext.SaveChanges();
        }

        private static (byte[], string) GenerateSalt()
        {
            // Generate salt
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var b64Salt = Convert.ToBase64String(salt);

            return (salt, b64Salt);
        }

        private static string CreateHash(string password, byte[] salt)
        {
            // Create hash from password and salt
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }

        private static string GenerateSecurityToken()
        {
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);

            return b64SecToken;
        }
    }
}
