using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwissLife.Slkv.Partner.ClientAppSample.Services;

namespace SwissLife.Slkv.Partner.ClientAppSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = Configuration["Auth0:ClientId"];
                    options.ClientSecret = Configuration["Auth0:ClientSecret"];
                    options.Authority = $"https://{Configuration["Auth0:Domain"]}";
                    options.ResponseType = "code id_token";
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("slkv_partner_access");
                    // options.Scope.Add("slkv_process_breaksalary");
                    // options.Scope.Add("slkv_process_changecontractaddress");
                    // options.Scope.Add("slkv_process_changepensionplan");
                    // options.Scope.Add("slkv_process_changepersonaddress");
                    // options.Scope.Add("slkv_process_changepersondata");
                    options.Scope.Add("slkv_process_changesalary");
                    // options.Scope.Add("slkv_process_entry");
                    // options.Scope.Add("slkv_process_leaving");
                    // options.Scope.Add("slkv_process_contractconnect");
                    // options.Scope.Add("slkv_process_iam");
                    // options.Scope.Add("slkv_process_retirement");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                });

            services.AddControllersWithViews();
            services.AddTransient<ISlkvPartnerApiClient, SlkvPartnerApiClient>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private static void CheckSameSite(
            HttpContext httpContext,
            CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        private static bool DisallowsSameSiteNone(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return false;
            }

            if (userAgent.Contains("CPU iPhone OS 12") ||
                userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") &&
                userAgent.Contains("Safari"))
            {
                return true;
            }

            if (userAgent.Contains("Chrome/5") ||
                userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }
    }
}
