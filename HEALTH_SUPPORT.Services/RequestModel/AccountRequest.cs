using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public static class AccountRequest
    {
        public class CreateAccountModel
        {
            [Required(ErrorMessage = "Thiếu tên tài khoản!")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Thiếu họ và tên!")]
            public string Fullname { get; set; }

            [Required(ErrorMessage = "Thiếu Email!")]
            [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ!")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Thiếu số điện thoại!")]
            [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ!")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Thiếu địa chỉ!")]
            public string Address { get; set; }

            [Required]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!")]
            public string PasswordHash { get; set; }
            [Required]
            [Compare("PasswordHash", ErrorMessage = "Mật khẩu nhập lại không khớp!")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Thiếu vai trò!")]
            public string RoleName { get; set; }

        }

        public class UpdateAccountModel
        {
            [Required]
            public Guid AccountId { get; set; }

            public string? Username { get; set; }
            public string? Fullname { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? PasswordHash { get; set; }
            public string? PasswordSalt { get; set; }
        }

        public class UpdatePasswordModel
        {
            public Guid AccountId { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class LoginRequestModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class VerificationEmailRequest
        {
            public Guid AccountId { get; set; }
            public string VerificationCode { get; set; }
        }

        public class OtpRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }

        public class UploadAvatarModel
        {
            public IFormFile File { get; set; }
        }
    }
}
