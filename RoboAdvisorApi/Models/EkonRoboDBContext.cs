using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RoboAdvisorApi.Models
{
    public partial class EkonRoboDBContext : DbContext
    {
        public EkonRoboDBContext()
        {
        }

        public EkonRoboDBContext(DbContextOptions<EkonRoboDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<BasketCategory> BasketCategory { get; set; }
        public virtual DbSet<Exceptions> Exceptions { get; set; }
        public virtual DbSet<Logins> Logins { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<RebalanceHistory> RebalanceHistory { get; set; }
        public virtual DbSet<RecordLogs> RecordLogs { get; set; }
        public virtual DbSet<RiskCategories> RiskCategories { get; set; }
        public virtual DbSet<SpecialBasketStocks> SpecialBasketStocks { get; set; }
        public virtual DbSet<SpecialBaskets> SpecialBaskets { get; set; }
        public virtual DbSet<SurveyQuestions> SurveyQuestions { get; set; }
        public virtual DbSet<SymbolData> SymbolData { get; set; }
        public virtual DbSet<Symbols> Symbols { get; set; }
        public virtual DbSet<TemplateBasketBackTests> TemplateBasketBackTests { get; set; }
        public virtual DbSet<TemplateBasketStocks> TemplateBasketStocks { get; set; }
        public virtual DbSet<TemplateBaskets> TemplateBaskets { get; set; }
        public virtual DbSet<UserBasketStocks> UserBasketStocks { get; set; }
        public virtual DbSet<UserBaskets> UserBaskets { get; set; }
        public virtual DbSet<UserCategoryHistory> UserCategoryHistory { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer("Server=192.168.250.10,1433\\SQLEXPRESS;Database=EkonRoboDBFon;User Id=sa;Password=Fidelio06;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answers>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.SurveyId).HasColumnName("SurveyID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Answers_Users");
            });

            modelBuilder.Entity<BasketCategory>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__BasketCa__FBDF78E90F7F28A3");

                entity.Property(e => e.RecordDate).HasColumnType("date");

                entity.Property(e => e.UpDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Exceptions>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.Application).HasMaxLength(100);

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Logins>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.Explanation).HasMaxLength(500);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(500);

                entity.Property(e => e.Port).HasMaxLength(500);

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.Property(e => e.Level).HasMaxLength(128);

                entity.Property(e => e.Properties).HasColumnType("xml");
            });

            modelBuilder.Entity<RebalanceHistory>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RebalanceHistory)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_RebalanceHistory_Users");
            });

            modelBuilder.Entity<RecordLogs>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.Application).HasMaxLength(100);

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<RiskCategories>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.Img)
                    .HasColumnName("img")
                    .HasColumnType("image");

                entity.Property(e => e.Imgext).HasColumnName("imgext");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.UpDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SpecialBasketStocks>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.RecordUserId).HasColumnName("RecordUserID");

                entity.Property(e => e.SpecialBasketId).HasColumnName("SpecialBasketID");

                entity.Property(e => e.SymbolId).HasColumnName("SymbolID");
            });

            modelBuilder.Entity<SpecialBaskets>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.RecordUserId).HasColumnName("RecordUserID");
            });

            modelBuilder.Entity<SurveyQuestions>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK_SurveryQuestions");

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.CrDate).HasColumnType("datetime");

                entity.Property(e => e.UpDate)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SymbolData>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK_FundData");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Symbols>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.Sym)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateBasketBackTests>(entity =>
            {
                entity.HasKey(e => e.BacktestId)
                    .HasName("PK_Backtests");

                entity.Property(e => e.BacktestId).HasColumnName("BacktestID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.TemplateBasketId).HasColumnName("TemplateBasketID");
            });

            modelBuilder.Entity<TemplateBasketStocks>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.RecordUserId).HasColumnName("RecordUserID");

                entity.Property(e => e.SymbolId).HasColumnName("SymbolID");

                entity.Property(e => e.TemplateBasketId).HasColumnName("TemplateBasketID");
            });

            modelBuilder.Entity<TemplateBaskets>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.RecordUserId).HasColumnName("RecordUserID");
            });

            modelBuilder.Entity<UserBasketStocks>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK_BasketStocks");

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.LastChange).HasColumnType("datetime");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.SymbolId).HasColumnName("SymbolID");

                entity.Property(e => e.UpDate).HasColumnType("datetime");

                entity.Property(e => e.UserBasketId).HasColumnName("UserBasketID");
            });

            modelBuilder.Entity<UserBaskets>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.ContractAmount).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.SendSms).HasColumnName("SendSMS");

                entity.Property(e => e.TemplateBasketId).HasColumnName("TemplateBasketID");

                entity.Property(e => e.UpDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserBaskets)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserBaskets_Users");
            });

            modelBuilder.Entity<UserCategoryHistory>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK_User_Category_Hist");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.UpDate).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.UserCategoryHistory)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_User_Category_Hist_RiskCategories");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCategoryHistory)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_User_Category_Hist_Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Authorization).HasMaxLength(500);

                entity.Property(e => e.CrDate)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.CustomerNo).HasMaxLength(500);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Gsm)
                    .HasColumnName("GSM")
                    .HasMaxLength(50);

                entity.Property(e => e.Hash).HasMaxLength(500);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.NameSurname).HasMaxLength(500);

                entity.Property(e => e.PasswordHash).HasMaxLength(1024);

                entity.Property(e => e.PasswordSalt).HasMaxLength(1024);

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.Property(e => e.Tckn)
                    .HasColumnName("TCKN")
                    .HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(500);

                entity.Property(e => e.UpDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
