using UsersCanLogIn.API.Controllers.Filters;
using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.DAL;
using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.DAL.Repositories;
using UsersCanLogIn.API.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace UsersCanLogIn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<UserRequestDTO>()
                .Bind(Configuration.GetSection("AdminUser"))
                .ValidateDataAnnotations();
            services.AddOptions<SmtpConfig>()
                .Bind(Configuration.GetSection("Smtp"))
                .ValidateDataAnnotations();

            LoadDbContexts(services);

            services.AddControllers(options =>
            {
                options.UseGeneralRoutePrefix("api/v{version:apiVersion}");
                options.Filters.Add(typeof(AuthenticationFilter));
                options.Filters.Add(typeof(InvalidModelStateFilter));
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IResponseObjectFactory, ResponseObjectFactory>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IDALInit, DALInit>();
            services.AddSingleton(AutoMapperConfig.GetConfig().CreateMapper());
            services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
            services.AddSingleton<IMailer, Mailer>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Program.AppName, Version = "v1" });
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.SlidingExpiration = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.AppName} v1"));
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });

            InitDb(app.ApplicationServices);
        }

        private void LoadDbContexts(IServiceCollection services)
        {
            DbType dbType = (DbType)Enum.Parse(typeof(DbType), Configuration["Database:Type"]);
            Action<DbContextOptionsBuilder> dbContextOptionsBuilder = GetDbContextOptionsBuilder(dbType);
            services.AddDbContext<UserContext>(dbContextOptionsBuilder);
            services.AddDbContext<VerificationTokenContext>(dbContextOptionsBuilder);
            services.AddDbContext<PasswordResetTokenContext>(dbContextOptionsBuilder);
        }

        private Action<DbContextOptionsBuilder> GetDbContextOptionsBuilder(DbType dbType)
        {
            void Action(DbContextOptionsBuilder opt)
            {
                switch (dbType)
                {
                    case DbType.SQLite:
                        opt.UseSqlite(Configuration.GetConnectionString("SQLite"));
                        break;
                    case DbType.SQLServer:
                        opt.UseSqlServer(Configuration.GetConnectionString("SQLServer"));
                        break;
                    default:
                        opt.UseInMemoryDatabase(Program.AppName);
                        break;
                }
            }
            return Action;
        }

        private void InitDb(IServiceProvider serviceProvider)
        {
            using var serviceScope = serviceProvider.CreateScope();
            var DALInit = (IDALInit)serviceScope.ServiceProvider.GetService(typeof(IDALInit));
            DALInit.Init();
        }
    }

    public enum DbType { InMemory, SQLite, SQLServer }
}
