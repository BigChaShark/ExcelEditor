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

    public virtual DbSet<LogeGroup> LogeGroups { get; set; }

    public virtual DbSet<LogeTempMaster> LogeTempMasters { get; set; }

    public virtual DbSet<LogeTempOffline> LogeTempOfflines { get; set; }

    public virtual DbSet<SubZone> SubZones { get; set; }

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
