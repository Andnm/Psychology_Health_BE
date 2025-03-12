using HEALTH_SUPPORT.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SubscriptionData> SubscriptionDatas { get; set; }
        public DbSet<SubscriptionProgress> SubscriptionProgresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SurveyType> SurveyTypes { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }
        public DbSet<SurveyResults> SurveyResults { get; set; }
        public DbSet<AccountSurvey> AccountSurveys { get; set; }
        public DbSet<Appointment> Appointments { get; set; }



        private readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("2a5f5c96-cb79-40d4-a604-d484b7041e7f"), Name = "Manager"},
                new Role { Id = Guid.Parse("7d9d691a-58dc-48fd-9204-ffe02c4fd0fd"), Name = "Student" },
                new Role { Id = Guid.Parse("b6286c3e-1e4b-41ce-81e5-cc9a27ffe2e7"), Name = "Parent" },
                new Role { Id = Guid.Parse("5fff93bf-2324-425b-8f04-6a80af3bb0d3"), Name = "Psychologist" }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = Guid.Parse("5b0884a0-0067-49f5-b3be-a29ef58aa70c"),
                    UseName = "Manager1",
                    Fullname = "Manager1 nè",
                    Email = "admin@example.com",
                    Phone = "0123456789",
                    Address = "123 Admin Street",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    CreateAt = DateTimeOffset.UtcNow,
                    LoginDate = DateTimeOffset.UtcNow,
                    RoleId = Guid.Parse("2a5f5c96-cb79-40d4-a604-d484b7041e7f")
                },
                new Account
                {
                    Id = Guid.Parse("dad2a80f-70e4-49f6-b3c5-3c1eedf525e4"),
                    UseName = "Manager2",
                    Fullname = "Manager2 nè",
                    Email = "admin2@example.com",
                    Phone = "0123456789",
                    Address = "123 Admin Street",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@456"),
                    CreateAt = DateTimeOffset.UtcNow,
                    LoginDate = DateTimeOffset.UtcNow,
                    RoleId = Guid.Parse("2a5f5c96-cb79-40d4-a604-d484b7041e7f")
                }
            );
            modelBuilder.Entity<Account>().HasIndex(a => a.UseName).IsUnique();

            modelBuilder.Entity<SurveyType>().HasData(
                new SurveyType { Id = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8"), SurveyName = "Đánh giá lo âu (GAD-7)" },
                new SurveyType { Id = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e"), SurveyName = "Đánh giá trầm cảm (PHQ-9)" }
            );

            modelBuilder.Entity<Survey>().HasData(
                new Survey
                {
                    Id = Guid.Parse("418be23d-8db9-4b74-a86a-9402f246ea62"),
                    MaxScore = 21,
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new Survey
                {
                    Id = Guid.Parse("86deeb52-2ef9-47d9-8496-edac723ffbf7"),
                    MaxScore = 27,
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                }
            );

            modelBuilder.Entity<SurveyQuestion>().HasData(
                new SurveyQuestion
                {
                    Id = Guid.Parse("07cb8575-7b7d-4784-a6b2-9cc1543f0267"),
                    ContentQ = "Cảm thấy lo lắng, căng thẳng, hoặc bồn chồn.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("798b80ac-f766-4baf-aa01-dbebca21f98b"),
                    ContentQ = "Không thể dừng hoặc kiểm soát sự lo lắng.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("0998c44c-7a8d-41fd-8b3e-05d775ce4b27"),
                    ContentQ = "Lo lắng quá nhiều về những điều khác nhau.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("7b1cc80a-8c8f-4695-99eb-271c46b9c766"),
                    ContentQ = "Khó thư giãn.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("a1ce2d59-88d4-463d-8af2-f472b5771746"),
                    ContentQ = "Bồn chồn đến mức khó ngồi yên.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("a9ffedd5-3f8e-4d98-b230-ae2491045f0d"),
                    ContentQ = "Dễ cáu gắt hoặc bực mình.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("4f4736e4-ecbe-4bdc-a9cf-cac7db918aae"),
                    ContentQ = "Cảm giác như có điều gì đó khủng khiếp sẽ xảy ra.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b23f0870-f5d9-463f-8ffc-98a133da47e8")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("59d5f97c-11b1-40d3-a69c-6c3700ff2c7d"),
                    ContentQ = "Ít hứng thú hoặc niềm vui khi làm mọi việc.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("37a557b5-c10a-4062-b7e5-0b2f4e02636e"),
                    ContentQ = "Cảm thấy buồn, chán nản, hoặc tuyệt vọng.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("416fcdf0-e6e6-4e76-9e3a-cb80b9a57a49"),
                    ContentQ = "Khó ngủ hoặc ngủ quá nhiều.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("a36264c0-9ad8-4205-811b-a867d2c7966d"),
                    ContentQ = "Mệt mỏi hoặc thiếu năng lượng.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("fdce1e6a-d8dc-4ddc-bd63-9f9acf666467"),
                    ContentQ = "Chán ăn hoặc ăn quá nhiều.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("0ff7226b-02f5-45dd-b3a6-f3bb2c804c5d"),
                    ContentQ = "Cảm giác tệ về bản thân, hoặc cảm thấy mình là người thất bại.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("8f0a5958-04ad-403a-bbad-ea476f5c2cc5"),
                    ContentQ = "Khó tập trung vào công việc hoặc khi đọc báo/xem TV.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("3f0dce08-ebbd-4872-b9dc-d3de0b66279f"),
                    ContentQ = "Cử động hoặc nói chậm hơn bình thường, hoặc ngược lại quá bồn chồn.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                },
                new SurveyQuestion
                {
                    Id = Guid.Parse("41ecf897-7bfa-4892-abb8-eee9aacc7eb5"),
                    ContentQ = "Suy nghĩ về việc tự làm hại bản thân hoặc muốn chết.",
                    CreateAt = DateTimeOffset.UtcNow,
                    SurveyTypeId = Guid.Parse("b24fc1c5-ff98-4767-adb6-56b57f91b22e")
                }
            );

            modelBuilder.Entity<SurveyAnswer>().HasData(
                new SurveyAnswer
                {
                    Id = Guid.Parse("28dee921-db63-45c3-87a3-bdf0a263370d"),
                    Content = "Không hề",
                    Point = 0,
                    CreateAt = DateTimeOffset.UtcNow
                },
                new SurveyAnswer
                {
                    Id = Guid.Parse("66d927b9-e3c2-47f3-8aa9-82260d1579cd"),
                    Content = "Vài ngày",
                    Point = 1,
                    CreateAt = DateTimeOffset.UtcNow
                },
                new SurveyAnswer
                {
                    Id = Guid.Parse("91c14006-832a-41dd-8d03-e7950cf54347"),
                    Content = "Hơn một nửa số ngày",
                    Point = 2,
                    CreateAt = DateTimeOffset.UtcNow
                },
                new SurveyAnswer
                {
                    Id = Guid.Parse("f7095907-611f-4ed9-b622-5df984f48175"),
                    Content = "Gần như mỗi ngày",
                    Point = 3,
                    CreateAt = DateTimeOffset.UtcNow
                }
            );

            //Account-survey(m-m: AccountSurvey)
            modelBuilder.Entity<Account>().HasMany(s => s.AccountSurveys).WithOne(a => a.Account).HasForeignKey(s => s.AccountId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Survey>().HasMany(s => s.AccountSurveys).WithOne(a => a.Survey).HasForeignKey(s => s.SurveyId).OnDelete(DeleteBehavior.Restrict);

            //Survey-SurveyResults(1-m)
            modelBuilder.Entity<Survey>().HasMany(s => s.SurveyResults).WithOne(a => a.Survey).HasForeignKey(s => s.SurveyId).OnDelete(DeleteBehavior.Restrict);

            //SurveyResults-AccountSurvey(m-1)
            modelBuilder.Entity<AccountSurvey>().HasMany(s => s.SurveyResults).WithOne(a => a.AccountSurvey).HasForeignKey(s => s.AccountSurveyId).OnDelete(DeleteBehavior.Restrict);

            //Survey-SurveyQuestion(m-m)
            modelBuilder.Entity<Survey>().HasMany(s => s.SurveyQuestions).WithMany(a => a.Surveys).UsingEntity(j => j.ToTable("SurveyQuestionSurvey"));

            //SurveyQuestion-SurveyAnswer(m-m)
            modelBuilder.Entity<SurveyQuestion>().HasMany(q => q.SurveyAnswers).WithMany(a => a.SurveyQuestions).UsingEntity(j => j.ToTable("SurveyQuestionAnswer"));

            //SurveyType-Survey(1-m)
            modelBuilder.Entity<SurveyType>().HasMany(s => s.Surveys).WithOne(a => a.SurveyType).HasForeignKey(s => s.SurveyTypeId).OnDelete(DeleteBehavior.Restrict);

            //SurveyType-SurveyQuestion(1-m)
            modelBuilder.Entity<SurveyType>().HasMany(s => s.SurveyQuestions).WithOne(a => a.SurveyType).HasForeignKey(s => s.SurveyTypeId).OnDelete(DeleteBehavior.Restrict);

            //account - appointment
            modelBuilder.Entity<Account>().HasMany(t => t.Appointments).WithOne(a => a.Account).HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Restrict);

            //Psychologist - appointment
            modelBuilder.Entity<Psychologist>().HasMany(t => t.Appointments).WithOne(a => a.Psychologist).HasForeignKey(t => t.PsychologistId).OnDelete(DeleteBehavior.Restrict);

            //account - Psychologist(m-m)
            modelBuilder.Entity<Account>().HasMany(s => s.HealthDatas).WithOne(a => a.Account).HasForeignKey(s => s.AccountId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Psychologist>().HasMany(s => s.HealthDatas).WithOne(a => a.Psychologist).HasForeignKey(s => s.PsychologistId).OnDelete(DeleteBehavior.Restrict);

            //Psychologist - SubscriptionData(1 -n)
            modelBuilder.Entity<Psychologist>().HasMany(s => s.SubscriptionDatas).WithOne(a => a.Psychologists).HasForeignKey(s => s.PsychologistId).OnDelete(DeleteBehavior.Restrict);

            //order - transaction(1-m)
            modelBuilder.Entity<Order>().HasMany(o => o.Transaction).WithOne(t => t.Order).HasForeignKey(o => o.OrderId).OnDelete(DeleteBehavior.Restrict);

            //order - subscription(m-1)
            modelBuilder.Entity<SubscriptionData>().HasMany(o => o.Orders).WithOne(d => d.SubscriptionData).HasForeignKey(o => o.SubscriptionDataId).OnDelete(DeleteBehavior.Restrict);

            //order - account(m-1)
            modelBuilder.Entity<Account>().HasMany(o => o.Orders).WithOne(d => d.Accounts).HasForeignKey(o => o.AccountId).OnDelete(DeleteBehavior.Restrict);

            //Category - SubscriptionData(1-m)
            modelBuilder.Entity<Category>().HasMany(d => d.SubscriptionDatas).WithOne(c => c.Category).HasForeignKey(d => d.CategoryId).OnDelete(DeleteBehavior.Restrict);

            // SubscriptionProgress - Order (m-1)
            modelBuilder.Entity<Order>().HasMany(p => p.SubscriptionProgresses).WithOne(o => o.Order).HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Restrict);
        }

    }
}
