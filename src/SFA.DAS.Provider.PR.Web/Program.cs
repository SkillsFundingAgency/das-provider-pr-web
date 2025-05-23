using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.Provider.PR.Web.AppStart;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Validators;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.LoadConfiguration(builder.Services);

builder.Services
    .Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    })
    .AddApplicationInsightsTelemetry()
    .AddLogging(builder =>
    {
        builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
        builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
    })
    .AddDataProtection(configuration)
    .AddHttpContextAccessor()
    .AddServiceRegistrations(configuration)
    .AddSession(configuration)
    .AddHealthChecks(configuration)
    .AddAuthentication(configuration)
    .AddValidatorsFromAssembly(typeof(SearchByEmailSubmitModelValidator).Assembly)
    .AddAuthorizationServicePolicies()
    .AddProviderUiServiceRegistration(configuration);

builder.Services
    .Configure<MvcViewOptions>(viewOptions => viewOptions.HtmlHelperOptions.CheckBoxHiddenInputRenderMode = CheckBoxHiddenInputRenderMode.None)
    .Configure<RouteOptions>(options => { options.LowercaseUrls = false; })
    .AddMvc(options =>
    {
        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
    })
    .AddSessionStateTempDataProvider()
    .SetDefaultNavigationSection(NavigationSection.Relationships)
    .EnableGoogleAnalytics();

#if DEBUG
builder.Services.AddControllersWithViews().AddControllersAsServices().AddRazorRuntimeCompilation();
#endif


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app
        .UseHsts()
        .UseStatusCodePagesWithReExecute("/error/{0}")
        .UseExceptionHandler("/error");
}

app
    .UseHealthChecks("/health",
        new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        })
    .UseHealthChecks("/ping",
        new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        })
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseCookiePolicy()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseSession()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");
    });

await app.RunAsync();
