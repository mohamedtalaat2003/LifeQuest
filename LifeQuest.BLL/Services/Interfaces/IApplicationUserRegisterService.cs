using LifeQuest.DAL.Models;
using LifeQuest.BLL.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IApplicationUserRegisterService
    {
        Task<bool> IsUnique(string UserName);
        Task<ApplicationUser> GetByNameAsync(string UserName);
        Task<(ApplicationUser user, List<string> error)> CreateAsync(RegisterationDTO applicationUser, string PlainPassword);
        Task<IEnumerable<SelectListItem>> Countries();
    }
}
