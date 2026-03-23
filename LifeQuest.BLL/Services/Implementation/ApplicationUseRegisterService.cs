using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.BLL.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LifeQuest.BLL.Services.Implementation
{
    public class ApplicationUseRegisterService : IApplicationUserRegisterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSenderService _emailSender;

        public ApplicationUseRegisterService(UserManager<ApplicationUser> userManager, IEmailSenderService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<bool> IsUnique(string UserName)
        {
            var user = await GetByNameAsync(UserName);
            if (user != null) return false;

            return true;
        }

        public async Task<ApplicationUser> GetByNameAsync(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            return user;
        }

        public async Task<(ApplicationUser user, List<string> error)> CreateAsync(RegisterationDTO AppUser, string PlainPassword)
        {
            var ExistingUser = await GetByNameAsync(AppUser.UserName);

            if (ExistingUser != null)
            {
                return (null, new List<string> { "Username already exists" });
            }

            var NewUser = new ApplicationUser
            {
                Name = AppUser.Name,
                UserName = AppUser.UserName,
                Email = AppUser.Email,
                Country = AppUser.Country
            };

            var result = await _userManager.CreateAsync(NewUser, PlainPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (null, errors);
            }

            return (NewUser, new List<string>());
        }

        public async Task<IEnumerable<SelectListItem>> Countries()
        {
            var countries = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(c => new RegionInfo(c.Name).EnglishName)
                .Distinct()
                .OrderBy(c => c)
                .Select(c => new SelectListItem { Text = c, Value = c })
                .ToList();

            return await Task.FromResult(countries);
        }
    }
}
