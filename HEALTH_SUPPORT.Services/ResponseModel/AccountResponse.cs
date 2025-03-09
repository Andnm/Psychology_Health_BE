using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public static class AccountResponse
    {
        public record GetAccountsModel(
            Guid Id,
            string UserName,
            string Fullname,
            string Email, 
            string Phone, 
            string Address,
            string PasswordHash,
            string RoleName
        );

        public class LoginResponseModel
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
            public string RoleName { get; set; }
            public bool IsEmailVerified { get; set; }
        }

        public class AvatarResponseModel
        {
            public string AvatarUrl { get; set; }
        }
    }
}
