using LifeQuest.DAL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LifeQuest.PL.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);

                if (IsAjaxRequest(context))
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
                else
                {
                    SetTempData(context, "ErrorMessage", ex.Message);
                    context.Response.Redirect("/Home/Error");
                }
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation: {Message}", ex.Message);

                if (IsAjaxRequest(context))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
                else
                {
                    SetTempData(context, "ErrorMessage", ex.Message);
                    context.Response.Redirect(context.Request.Headers["Referer"].ToString() ?? "/");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                if (IsAjaxRequest(context))
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
            }
        }

        private static bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private static void SetTempData(HttpContext context, string key, string value)
        {
            var tempDataFactory = context.RequestServices.GetService<ITempDataDictionaryFactory>();
            if (tempDataFactory != null)
            {
                var tempData = tempDataFactory.GetTempData(context);
                tempData[key] = value;
                tempData.Save();
            }
        }
    }
}
