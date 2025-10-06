using Project.Areas.Identity.Data;
using Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Project.Repository
{
    public interface IAccountRepository
    {

        public Task<List<ApplicationUser>> ShowUserAsync();
        public Task<ApplicationUser> GetUesrByEmailAsync(string email);

       /* public Task<IdentityResult> LockUserByEmailAsync(string UserName);
        public Task<IdentityResult> UnlockUserByEmailAsync(string UserName);
        public Task<IdentityResult> ConfirmAccountAsync(string email);
        public Task<IdentityResult> RestPassAsync(string email, string newPass);
        public Task<IdentityResult> ChangePass(string email, string NewPass, string CurrentPass);*/
    }
}
