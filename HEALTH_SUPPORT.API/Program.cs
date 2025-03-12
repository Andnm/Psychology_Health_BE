
using HEALTH_SUPPORT.Repositories;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.Implementations;
using HEALTH_SUPPORT.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace HEALTH_SUPPORT.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Đọc chuỗi cấu hình JWT từ appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            string secretKey = jwtSettings["SecretKey"];
            string issuer = jwtSettings["Issuer"];
            string audience = jwtSettings["Audience"];

            // 2. Đăng ký Authentication với JwtBearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // để dev local cho nhanh, prod thì nên để true
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true, // kiểm tra token hết hạn
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Thêm Authorization
            builder.Services.AddAuthorization();

            // Thêm các service khác (Repositories, Services, v.v.)
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


       
            builder.Services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));

            // Thêm MemoryCache
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IEmailService, EmailService>();


            // Inject IWebHostEnvironment: giúp acc update ảnh đại diện
            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            // Register IService and Service
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAvatarRepository, AvatarRepository>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<ISurveyService, SurveyService>();
            builder.Services.AddScoped<ISurveyTypeService, SurveyTypeService>();
            builder.Services.AddScoped<ISurveyQuestionService, SurveyQuestionService>();
            builder.Services.AddScoped<ISurveyAnswerService, SurveyAnswerService>();
            builder.Services.AddScoped<IAccountSurveyService, AccountSurveyService>();
            builder.Services.AddScoped<ISurveyResultsService, SurveyResultService>();

            var app = builder.Build();

            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            // Lấy IWebHostEnvironment từ app.Services
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();

            // Debug thông tin môi trường
            Console.WriteLine($"Environment: {env.EnvironmentName}");
            Console.WriteLine($"WebRootPath: {env.WebRootPath}");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles(); // Cho phép truy cập ảnh đã upload

            app.MapControllers();

            app.Run();

           
        }
    }
}
