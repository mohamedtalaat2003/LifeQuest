using System.Security.Claims;
using LifeQuest.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LifeQuest.PL.Filters
{
    public class UserStatsActionFilter : IAsyncActionFilter
    {
        private readonly IUserProfileService _userProfileService;

        public UserStatsActionFilter(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    try
                    {
                        var profile = await _userProfileService.GetUserProfileAsync(userId);
                        if (profile != null)
                        {
                            var controller = context.Controller as Controller;
                            if (controller != null)
                            {
                                controller.ViewData["UserPoints"] = profile.TotalPoints.ToString();
                                controller.ViewData["UserLevel"] = $"{profile.LevelNumber - 1} - {profile.LevelName}";
                                controller.ViewData["ProfilePicture"] = profile.ProfilePictureUrl;
                            }
                        }
                    }
                    catch
                    {
                        // Silent fail if profile not found or service error during filter
                    }
                }
            }

            await next();
        }
    }
}
