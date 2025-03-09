using HEALTH_SUPPORT.Repositories;
using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<Account, Guid> _accountRepository;
        private readonly IBaseRepository<Role, Guid> _roleRepository;
        private readonly IConfiguration _configuration;

        private readonly IHostEnvironment _environment;
        private readonly IAvatarRepository _avatarRepository;

        public AccountService(IBaseRepository<Account, Guid> accountRepository, IBaseRepository<Role, Guid> roleRepository, IConfiguration configuration, IHostEnvironment environment)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task AddAccount(AccountRequest.CreateAccountModel model)
        {
            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _accountRepository.GetAll().AnyAsync(r => r.Email == model.Email);
            if (existingUser)
            {
                throw new Exception("Email đã được sử dụng.");
            }
            // Kiểm tra role có tồn tại không
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.Name == model.RoleName);
            if (role == null)
            {
                throw new Exception("Vui lòng chọn vai trò.");
            }
            // Kiểm tra mật khẩu nhập lại
            if (model.PasswordHash != model.ConfirmPassword)
            {
                throw new Exception("Mật khẩu nhập lại không khớp!");
            }
            // Mã hóa mật khẩu
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
            try
            {
                var acc = new Account()
                {
                    Id = Guid.NewGuid(),
                    UseName = model.UserName,
                    Fullname = model.Fullname,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    PasswordHash = passwordHash,
                    RoleId = role.Id,
                    CreateAt = DateTimeOffset.UtcNow,
                };
                await _accountRepository.Add(acc);
                await _accountRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<AccountResponse.GetAccountsModel?> GetAccountById(Guid id)
        {
            var account = await _accountRepository.GetAll().Include(a => a.Role).FirstOrDefaultAsync(a => a.Id == id);
            if (account == null || account.IsDeleted)
            {
                return null;
            }
            return new AccountResponse.GetAccountsModel(
                account.Id,
                account.UseName,
                account.Fullname,
                account.Email,
                account.Phone,
                account.Address,
                account.PasswordHash,
                account.Role?.Name ?? "Unknown"
            );
        }

        public async Task<List<AccountResponse.GetAccountsModel>> GetAccounts()
        {
            return await _accountRepository.GetAll()
                .Where(a => !a.IsDeleted)
                .AsNoTracking()
                .Select(a => new AccountResponse.GetAccountsModel(
                    a.Id,
                    a.UseName,
                    a.Fullname,
                    a.Email,
                    a.Phone,
                    a.Address,
                    a.PasswordHash,
                    a.Role.Name
                ))
                .ToListAsync();
        }

        public async Task RemoveAccount(Guid id)
        {
            var account = await _accountRepository.GetById(id);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found");
            }

            account.IsDeleted = true;
            account.ModifiedAt = DateTimeOffset.UtcNow;

            await _accountRepository.Update(account);
            await _accountRepository.SaveChangesAsync();
        }

        public async Task UpdateAccount(Guid id, AccountRequest.UpdateAccountModel model)
        {
            try
            {
                var existedAcc = await _accountRepository.GetById(id);
                if (existedAcc is null)
                {
                    throw new Exception("Not exist account!");
                }

                //Tracking
                existedAcc.UseName = string.IsNullOrWhiteSpace(model.Username) ? existedAcc.UseName : model.Username;
                existedAcc.Fullname = string.IsNullOrWhiteSpace(model.Fullname) ? existedAcc.Fullname : model.Fullname;
                existedAcc.Email = string.IsNullOrWhiteSpace(model.Email) ? existedAcc.Email : model.Email;
                existedAcc.Phone = string.IsNullOrWhiteSpace(model.Phone) ? existedAcc.Phone : model.Phone;
                existedAcc.Address = string.IsNullOrWhiteSpace(model.Address) ? existedAcc.Address : model.Address;
                existedAcc.PasswordHash = string.IsNullOrWhiteSpace(model.PasswordHash) ? existedAcc.PasswordHash : model.PasswordHash;
                //AsNoTracking
                await _accountRepository.Update(existedAcc);
                await _accountRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> UpdatePassword(AccountRequest.UpdatePasswordModel model)
        {
            var account = await _accountRepository.GetById(model.AccountId);
            if (account == null || account.IsDeleted) throw new Exception("Tài khoản không tồn tại hoặc đã bị xóa.");

            bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.OldPassword, account.PasswordHash);
            if (!isOldPasswordCorrect)
            {
                throw new Exception("Mật khẩu cũ không chính xác.");
            }
            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            account.PasswordHash = newHashedPassword;
            await _accountRepository.Update(account);
            await _accountRepository.SaveChangesAsync();
            return true;
        }
        public async Task<AccountResponse.LoginResponseModel> ValidateLoginAsync(AccountRequest.LoginRequestModel model)
        {
            // Tìm account theo UserName và đảm bảo không bị xóa
            var account = await _accountRepository.GetAll()
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == model.Email && !a.IsDeleted);

            if (account == null || !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
                return null;


            account.LoginDate = DateTimeOffset.UtcNow;
            await _accountRepository.Update(account);

            // Trả về thông tin cần thiết qua DTO LoginResponseModel
            return new AccountResponse.LoginResponseModel
            {
                Id = account.Id,
                UserName = account.UseName,
                RoleName = account.Role?.Name ?? "Unknown",
                IsEmailVerified = account.IsEmailVerified
            };
        }

        public string GenerateJwtToken(AccountResponse.LoginResponseModel account)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            string secretKey = jwtSettings["SecretKey"];
            string issuer = jwtSettings["Issuer"];
            string audience = jwtSettings["Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Thêm claim RoleName vào token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, account.UserName),
                new Claim(ClaimTypes.Role, account.RoleName) // Claim chứa thông tin Role
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public async Task<bool> VerifyEmailAsync(string email)
        {
            var account = await _accountRepository.GetAll()
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email);
            if (account == null) return false;

            if (account.IsEmailVerified) return true;

            account.IsEmailVerified = true;
            await _accountRepository.Update(account);
            return true;
        }
        
        public async Task<AccountResponse.AvatarResponseModel> UploadAvatarAsync(Guid accountId, AccountRequest.UploadAvatarModel model)
        {
            var account = await _accountRepository.GetById(accountId);
            if (account == null) throw new Exception("Account not found");

            // Vì IHostEnvironment không có WebRootPath, cần tự tạo đường dẫn wwwroot
            string uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}_{model.File.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            string relativePath = $"/uploads/{fileName}";
            await _avatarRepository.UpdateAvatarAsync(accountId, relativePath);

            return new AccountResponse.AvatarResponseModel { AvatarUrl = relativePath };
        }

        public async Task<AccountResponse.AvatarResponseModel> UpdateAvatarAsync(Guid accountId, AccountRequest.UploadAvatarModel model)
        {
            var account = await _accountRepository.GetById(accountId);
            if (account == null) throw new Exception("Account not found");

            if (!string.IsNullOrEmpty(account.ImgUrl))
            {
                string oldFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot", account.ImgUrl.TrimStart('/'));
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
            }
            return await UploadAvatarAsync(accountId, model);
        }

        public async Task RemoveAvatarAsync(Guid accountId)
        {
            var account = await _accountRepository.GetById(accountId);
            if (account == null) throw new Exception("Account not found");

            if (!string.IsNullOrEmpty(account.ImgUrl))
            {
                string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", account.ImgUrl.TrimStart('/'));
                if (File.Exists(filePath)) File.Delete(filePath);

                await _avatarRepository.UpdateAvatarAsync(accountId, null);
            }
        }
    }
}
