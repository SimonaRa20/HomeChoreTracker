
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Mapping;
using HomeChoreTracker.Api.Repositories;
using HomeChoreTracker.Api.Services;
using HomeChoreTracker.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace HomeChoreTracker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection(AuthSettings.SectionName));
			builder.Services.Configure<EmailConfigServer>(builder.Configuration.GetSection(EmailConfigServer.SectionName));
			builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
			builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiPlayground", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer YOUR_TOKEN_HERE'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
            });

            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Test", builder =>
                {
                    builder.WithOrigins("https://localhost:7052/")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .SetIsOriginAllowed((x) => true)
                           .AllowCredentials();
                });
            });
            builder.Services.AddDbContext<HomeChoreTrackerDbContext>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
            
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IHomeChoreBaseRepository, HomeChoreBaseRepository>();
            builder.Services.AddScoped<IHomeRepository, HomeRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
			builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
			builder.Services.AddScoped<IForumRepository, ForumRepository>();
			builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            builder.Services.AddScoped<IHomeChoreRepository, HomeChoreRepository>();
            builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
            builder.Services.AddScoped<IGamificationRepository, GamificationRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IAvatarRepository, AvatarRepository>();
            builder.Services.AddScoped<IChallengeRepository, ChallengeRepository>();
            builder.Services.AddHostedService<GamificationService>();
			builder.Services.AddHostedService<NotificationService>();
			var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("Test");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
