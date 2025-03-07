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

            //Account-survey(m-m: AccountSurvey)
            modelBuilder.Entity<Account>().HasMany(s => s.AccountSurveys).WithOne(a => a.Account).HasForeignKey(s => s.AccountId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Survey>().HasMany(s => s.AccountSurveys).WithOne(a => a.Survey).HasForeignKey(s => s.SurveyId).OnDelete(DeleteBehavior.Restrict);

            //Survey-SurveyResults(1-m)
            modelBuilder.Entity<Survey>().HasMany(s => s.SurveyResults).WithOne(a => a.Survey).HasForeignKey(s => s.SurveyId).OnDelete(DeleteBehavior.Restrict);

            //SurveyResults-AccountSurvey(m-1)
            modelBuilder.Entity<AccountSurvey>().HasMany(s => s.SurveyResults).WithOne(a => a.AccountSurvey).HasForeignKey(s => s.AccountSurveyId).OnDelete(DeleteBehavior.Restrict);

            //Survey-SurveyQuestion(1-m)
            modelBuilder.Entity<Survey>().HasMany(s => s.SurveyQuestions).WithOne(a => a.Survey).HasForeignKey(s => s.SurveyId).OnDelete(DeleteBehavior.Restrict);

            //SurveyQuestion-SurveyAnswer(1-m)
            modelBuilder.Entity<SurveyQuestion>().HasMany(s => s.SurveyAnswers).WithOne(a => a.SurveyQuestion).HasForeignKey(s => s.QuestionId).OnDelete(DeleteBehavior.Restrict);

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
