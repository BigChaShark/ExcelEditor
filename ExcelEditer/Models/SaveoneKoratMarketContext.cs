using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExcelEditor.Models;

public partial class SaveoneKoratMarketContext : DbContext
{
    public SaveoneKoratMarketContext()
    {
    }

    public SaveoneKoratMarketContext(DbContextOptions<SaveoneKoratMarketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Loge> Loges { get; set; }

    public virtual DbSet<LogeCostPerDay> LogeCostPerDays { get; set; }

    public virtual DbSet<LogeGroup> LogeGroups { get; set; }

    public virtual DbSet<LogeTempMaster> LogeTempMasters { get; set; }

    public virtual DbSet<LogeTempOffline> LogeTempOfflines { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<ReservationLoge> ReservationLoges { get; set; }

    public virtual DbSet<ReservationLogeDetail> ReservationLogeDetails { get; set; }

    public virtual DbSet<ReservationLogeElectricityType> ReservationLogeElectricityTypes { get; set; }

    public virtual DbSet<ReservationLogeElectronicType> ReservationLogeElectronicTypes { get; set; }

    public virtual DbSet<SubZone> SubZones { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<UserOffline> UserOfflines { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=147.50.166.16;Database=SaveoneKoratMarket;User Id=sa;Password=9]kfoyfgayoF8ik;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<Loge>(entity =>
        {
            entity.ToTable("Loge");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.IsConner).HasDefaultValue(0);
            entity.Property(e => e.IsOpen).HasDefaultValue(1);
            entity.Property(e => e.IsRandom).HasDefaultValue(0);
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("numeric(9, 2)");

            entity.HasOne(d => d.LogeGroup).WithMany(p => p.Loges)
                .HasForeignKey(d => d.LogeGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loge_LogeGroup");
        });

        modelBuilder.Entity<LogeCostPerDay>(entity =>
        {
            entity.HasKey(e => new { e.SubZoneId, e.Day });

            entity.ToTable("LogeCostPerDay");

            entity.Property(e => e.Day)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Cost).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.Cost2).HasColumnType("numeric(9, 2)");

            entity.HasOne(d => d.SubZone).WithMany(p => p.LogeCostPerDays)
                .HasForeignKey(d => d.SubZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogeCostPerDay_SubZone");
        });

        modelBuilder.Entity<LogeGroup>(entity =>
        {
            entity.ToTable("LogeGroup");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.SubZone).WithMany(p => p.LogeGroups)
                .HasForeignKey(d => d.SubZoneId)
                .HasConstraintName("FK_LogeGroup_SubZone");

            entity.HasOne(d => d.Zone).WithMany(p => p.LogeGroups)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogeGroup_Zone");
        });

        modelBuilder.Entity<LogeTempMaster>(entity =>
        {
            entity.HasKey(e => new { e.LogeId, e.OpenCase }).HasName("PK_LogeTemp_Master_1");

            entity.ToTable("LogeTemp_Master");

            entity.Property(e => e.OpenCase).HasComment("1 = จันทร์ - พฤหัส , 2 = ศุกร์ - อาทิตย์");
            entity.Property(e => e.LogeName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Loge).WithMany(p => p.LogeTempMasters)
                .HasForeignKey(d => d.LogeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogeTemp_Master_Loge");
        });

        modelBuilder.Entity<LogeTempOffline>(entity =>
        {
            entity.HasKey(e => e.Seq);

            entity.ToTable("LogeTemp_Offline");

            entity.Property(e => e.Seq)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)");
            entity.Property(e => e.LogeName)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.Loge).WithMany(p => p.LogeTempOfflines)
                .HasForeignKey(d => d.LogeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogeTemp_Offline_Loge");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("Member");

            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.AddressNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.Box1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Box2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Box3)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Box4)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Box5)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EndVipZone2Date).HasColumnType("datetime");
            entity.Property(e => e.EndVipZone3Date).HasColumnType("datetime");
            entity.Property(e => e.EndVipZone5Date).HasColumnType("datetime");
            entity.Property(e => e.FacebookId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdCard)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IdCard2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ImageCoverShopUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImageHomeRegistrationUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImageIdCardUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImageLogoShopUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImagePersonUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.InstagramId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(1);
            entity.Property(e => e.IsAdmin).HasDefaultValue(0);
            entity.Property(e => e.IsBkk).HasColumnName("isBKK");
            entity.Property(e => e.IsDetechOneMoreIp).HasColumnName("IsDetechOneMoreIP");
            entity.Property(e => e.IsLoginOtp).HasColumnName("IsLoginOTP");
            entity.Property(e => e.IsOtpreservationLast4Idcard).HasColumnName("IsOTPReservationLast4IDCard");
            entity.Property(e => e.IsPdpa).HasColumnName("IsPDPA");
            entity.Property(e => e.IsResCon).HasDefaultValue(0);
            entity.Property(e => e.IsResConMax).HasDefaultValue(0);
            entity.Property(e => e.IsResConZone2).HasDefaultValue(0);
            entity.Property(e => e.IsShowKtb).HasColumnName("IsShowKTB");
            entity.Property(e => e.IsShowKtpregister)
                .HasDefaultValue(0)
                .HasColumnName("IsShowKTPRegister");
            entity.Property(e => e.IsShowScb).HasColumnName("IsShowSCB");
            entity.Property(e => e.IsShowScbregister)
                .HasDefaultValue(0)
                .HasColumnName("IsShowSCBRegister");
            entity.Property(e => e.IsVipstatusZone2).HasColumnName("IsVIPStatusZone2");
            entity.Property(e => e.IsVipstatusZone22).HasColumnName("IsVIPStatusZone2_2");
            entity.Property(e => e.IsVipstatusZone3).HasColumnName("IsVIPStatusZone3");
            entity.Property(e => e.IsVipstatusZone32).HasColumnName("IsVIPStatusZone3_2");
            entity.Property(e => e.IsVipstatusZone5).HasColumnName("IsVIPStatusZone5");
            entity.Property(e => e.IsVipstatusZone52).HasColumnName("IsVIPStatusZone5_2");
            entity.Property(e => e.IsVipzone1)
                .HasDefaultValue(0)
                .HasColumnName("IsVIPZone1");
            entity.Property(e => e.IsVipzone2)
                .HasDefaultValue(0)
                .HasColumnName("IsVIPZone2");
            entity.Property(e => e.IsVipzone3).HasColumnName("IsVIPZone3");
            entity.Property(e => e.IsVipzone4).HasColumnName("IsVIPZone4");
            entity.Property(e => e.IsVipzone5).HasColumnName("IsVIPZone5");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.LastedReserve).HasColumnType("datetime");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LockerNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LockerNo2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LockerNo3)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LockerNo4)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LockerNo5)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Moo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NickName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OtherImageNo).IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pin)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Postcode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Prefix)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegisterDate).HasColumnType("datetime");
            entity.Property(e => e.Remark).IsUnicode(false);
            entity.Property(e => e.Road)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ShopName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ShopType)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ShopType1)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ShopType_1");
            entity.Property(e => e.ShopType2)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ShopType_2");
            entity.Property(e => e.ShopType3)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ShopType_3");
            entity.Property(e => e.Soi)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SubDistrict)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Zone1).HasDefaultValue(0);
            entity.Property(e => e.Zone2).HasDefaultValue(0);
            entity.Property(e => e.Zone2Group)
                .HasDefaultValue(0)
                .HasColumnName("Zone2_Group");
            entity.Property(e => e.Zone3Group).HasColumnName("Zone3_Group");
            entity.Property(e => e.Zone5Group).HasColumnName("Zone5_Group");
        });

        modelBuilder.Entity<ReservationLoge>(entity =>
        {
            entity.ToTable("ReservationLoge");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.ElectricityAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.ElectronicAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.EstampAmount)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("EStampAmount");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.FullAreaAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.LogeAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.LogeName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PaymentFee).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.SavingAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.Member).WithMany(p => p.ReservationLoges)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservationLoge_Member");

            entity.HasOne(d => d.SubZone).WithMany(p => p.ReservationLoges)
                .HasForeignKey(d => d.SubZoneId)
                .HasConstraintName("FK_ReservationLoge_SubZone");

            entity.HasOne(d => d.Zone).WithMany(p => p.ReservationLoges)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK_ReservationLoge_Zone");
        });

        modelBuilder.Entity<ReservationLogeDetail>(entity =>
        {
            entity.HasKey(e => new { e.LogeId, e.ReservationDate }).HasName("PK_ReservationLogeDetail_1");

            entity.ToTable("ReservationLogeDetail");

            entity.Property(e => e.EstampId).HasColumnName("EStampId");
            entity.Property(e => e.Remark).IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Loge).WithMany(p => p.ReservationLogeDetails)
                .HasForeignKey(d => d.LogeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservationLogeDetail_Loge");

            entity.HasOne(d => d.ReservationLoge).WithMany(p => p.ReservationLogeDetails)
                .HasForeignKey(d => d.ReservationLogeId)
                .HasConstraintName("FK_ReservationLogeDetail_ReservationLoge");
        });

        modelBuilder.Entity<ReservationLogeElectricityType>(entity =>
        {
            entity.ToTable("ReservationLogeElectricityType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ReservationLogeElectronicType>(entity =>
        {
            entity.ToTable("ReservationLogeElectronicType");

            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SubZone>(entity =>
        {
            entity.ToTable("SubZone");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.GeneralClose).HasColumnType("datetime");
            entity.Property(e => e.GeneralOpen).HasColumnType("datetime");
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SaleGroup).HasDefaultValue(1);
            entity.Property(e => e.Vipclose)
                .HasColumnType("datetime")
                .HasColumnName("VIPClose");
            entity.Property(e => e.Vipopen)
                .HasColumnType("datetime")
                .HasColumnName("VIPOpen");

            entity.HasOne(d => d.Zone).WithMany(p => p.SubZones)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK_SubZone_Zone");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TranId);

            entity.ToTable("Transaction", tb => tb.HasComment("RPI = Request Payment Input\r\nRPO = Request Payment Output\r\nPRN = Payment result for HTTP post parameter\r\nPRS =  Payment result for respUrl (Silent Post)"));

            entity.HasIndex(e => e.TranCode, "IX_Transaction").IsUnique();

            entity.Property(e => e.AmountToPay).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.BypassPaymentStatusId).HasDefaultValue(1);
            entity.Property(e => e.CancleDate).HasColumnType("datetime");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.IsBillVat)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsChangeZone).HasColumnName("isChangeZone");
            entity.Property(e => e.IsDiscountRain).HasComment("null = ไม่มีส่วนลด | 1 = บันทึกส่วนลดฝนตก | 2 = บันทึกส่วนลดฝนตก (free day)");
            entity.Property(e => e.IsDiscountRainFreeDay)
                .HasComment("null = ไม่มีส่วนลด | 1 = บันทึกส่วนลดฝนตก | 2 = บันทึกส่วนลดฝนตก (free day)")
                .HasColumnName("IsDiscountRain_FreeDay");
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentEndPointUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PaymentReferenceCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PaymentResponseCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentResponseMessage)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RcptCode)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.RcptCode2)
                .HasMaxLength(14)
                .IsUnicode(false);
            entity.Property(e => e.RcptNo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RcptType).HasDefaultValue(4);
            entity.Property(e => e.ReservationsDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationsResult)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ReservationsRound).HasColumnType("datetime");
            entity.Property(e => e.TranCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ChangeReservationSubZone).WithMany(p => p.TransactionChangeReservationSubZones)
                .HasForeignKey(d => d.ChangeReservationSubZoneId)
                .HasConstraintName("FK_Transaction_SubZone1");

            entity.HasOne(d => d.Loge).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.LogeId)
                .HasConstraintName("FK_Transaction_Loge");

            entity.HasOne(d => d.Member).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Member");

            entity.HasOne(d => d.ReservationLogeElectricityType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ReservationLogeElectricityTypeId)
                .HasConstraintName("FK_Transaction_ReservationLogeElectricityType");

            entity.HasOne(d => d.ReservationLogeElectronicType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ReservationLogeElectronicTypeId)
                .HasConstraintName("FK_Transaction_ReservationLogeElectronicType");

            entity.HasOne(d => d.ReservationLoge).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ReservationLogeId)
                .HasConstraintName("FK_Transaction_ReservationLoge");

            entity.HasOne(d => d.ReservationSubZone).WithMany(p => p.TransactionReservationSubZones)
                .HasForeignKey(d => d.ReservationSubZoneId)
                .HasConstraintName("FK_Transaction_SubZone");
        });

        modelBuilder.Entity<UserOffline>(entity =>
        {
            entity.ToTable("User_Offline");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.ElectricityAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.ElectronicAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.LogeAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.LogeId)
                .IsUnicode(false)
                .HasColumnName("LogeID");
            entity.Property(e => e.LogeName).IsUnicode(false);
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.SubZoneId).HasColumnName("SubZoneID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.UserOfflineId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserOfflineID");
            entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.ToTable("Zone");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
