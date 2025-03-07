using System;
using System.Net;
using System.Net.Mail;
using HEALTH_SUPPORT.Services.IServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public EmailService(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public void GenerateOtp(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var expiresAt = TimeSpan.FromMinutes(5); // OTP có hiệu lực trong 5 phút

            // Lưu vào MemoryCache với thời gian hết hạn tự động
            _cache.Set(email, otp, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiresAt
            });

            // Gửi email OTP
            SendEmail(email, "Mã OTP của bạn", $"Mã OTP của bạn là: {otp}");
        }

        private void SendEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            smtpClient.Send(mailMessage);
        }

        public bool VerifyOtp(string email, string otp)
        {
            if (!_cache.TryGetValue(email, out string storedOtp) || !storedOtp.Equals(otp.Trim()))
            {
                return false;
            }

            // Xác thực thành công, lưu trạng thái đã xác thực OTP
            _cache.Set($"OTP_Verified_{email}", true, TimeSpan.FromHours(1));

            // Xóa OTP sau khi xác thực thành công để tránh sử dụng lại
            _cache.Remove(email);

            return true;
        }
    }
}
