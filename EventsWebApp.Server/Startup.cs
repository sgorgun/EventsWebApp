using EventsWebApp.Application.Interfaces;
using EventsWebApp.Server.Mapper;
using EventsWebApp.Application.Services;
using EventsWebApp.Infrastructure.Handlers;
using EventsWebApp.Infrastructure.Repositories;
using EventsWebApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using EventsWebApp.Infrastructure.UnitOfWork;
using EventsWebApp.Application.Validators;
using EventsWebApp.Server.Extensions;
using EventsWebApp.Server.ExceptionsHandler;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace EventsWebApp.Server
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
            string connection = Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("No database connection string");

            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAttendeeRepository, AttendeeRepository>();
            services.AddScoped<ISocialEventRepository, SocialEventRepository>();
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<UserValidator>();
            services.AddScoped<SocialEventValidator>();
            services.AddScoped<AttendeeValidator>();

            services.AddScoped<UserService>();
            services.AddScoped<SocialEventService>();
            services.AddScoped<AttendeeService>();

            services.AddApiAuthentication(Configuration);

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AppMappingProfile());
            });

            services.AddAutoMapper(typeof(AppMappingProfile));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always, 
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionHandler();

        }
        
    }
}