using HEALTH_SUPPORT.Repositories.Abstraction;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.IServices
{
    public interface IAccountService
    {
        Task<List<AccountResponse.GetAccountsModel>> GetAccounts();
        Task<AccountResponse.GetAccountsModel?> GetAccountById(Guid id);
        Task AddAccount(AccountRequest.CreateAccountModel model);
        Task UpdateAccount(Guid id, AccountRequest.UpdateAccountModel model);
        Task RemoveAccount(Guid id);

        Task<AccountResponse.LoginResponseModel> ValidateLoginAsync(AccountRequest.LoginRequestModel model);
        string GenerateJwtToken(AccountResponse.LoginResponseModel account);

    }
}
