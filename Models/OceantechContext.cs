using Microsoft.EntityFrameworkCore;

namespace Tiennthe171977_Oceanteach.Models;

public partial class OceantechContext : DbContext
{
    public OceantechContext()
    {
    }

    public OceantechContext(DbContextOptions<OceantechContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DanToc> DanTocs { get; set; }

    public virtual DbSet<DanhMucHuyen> DanhMucHuyens { get; set; }

    public virtual DbSet<DanhMucTinh> DanhMucTinhs { get; set; }

    public virtual DbSet<DanhMucXa> DanhMucXas { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<NgheNghiep> NgheNghieps { get; set; }

    public virtual DbSet<VanBang> VanBangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DanToc>(entity =>
        {
            entity.HasKey(e => e.DanTocId).HasName("PK__DanToc__F44CC5B75186D570");

            entity.ToTable("DanToc");

            entity.Property(e => e.DanTocId).HasColumnName("DanTocID");
            entity.Property(e => e.TenDanToc).HasMaxLength(100);
        });

        modelBuilder.Entity<DanhMucHuyen>(entity =>
        {
            entity.HasKey(e => e.HuyenId).HasName("PK__DanhMucH__D8F9C322BD6A6F2F");

            entity.ToTable("DanhMucHuyen");

            entity.Property(e => e.HuyenId).HasColumnName("HuyenID");
            entity.Property(e => e.TenHuyen).HasMaxLength(100);
            entity.Property(e => e.TinhId).HasColumnName("TinhID");

            entity.HasOne(d => d.Tinh).WithMany(p => p.DanhMucHuyens)
                .HasForeignKey(d => d.TinhId)
                .HasConstraintName("FK__DanhMucHu__TinhI__398D8EEE");
        });

        modelBuilder.Entity<DanhMucTinh>(entity =>
        {
            entity.HasKey(e => e.TinhId).HasName("PK__DanhMucT__823CCF8C146AB860");

            entity.ToTable("DanhMucTinh");

            entity.Property(e => e.TinhId).HasColumnName("TinhID");
            entity.Property(e => e.TenTinh).HasMaxLength(100);
        });

        modelBuilder.Entity<DanhMucXa>(entity =>
        {
            entity.HasKey(e => e.XaId).HasName("PK__DanhMucX__B913861EB8175506");

            entity.ToTable("DanhMucXa");

            entity.Property(e => e.XaId).HasColumnName("XaID");
            entity.Property(e => e.HuyenId).HasColumnName("HuyenID");
            entity.Property(e => e.TenXa).HasMaxLength(100);

            entity.HasOne(d => d.Huyen).WithMany(p => p.DanhMucXas)
                .HasForeignKey(d => d.HuyenId)
                .HasConstraintName("FK__DanhMucXa__Huyen__3C69FB99");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1F23DF680");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("CCCD");
            entity.Property(e => e.DanTocId).HasColumnName("DanTocID");
            entity.Property(e => e.DiaChiCuThe).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.HuyenId).HasColumnName("HuyenID");
            entity.Property(e => e.NgheNghiepId).HasColumnName("NgheNghiepID");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TinhId).HasColumnName("TinhID");
            entity.Property(e => e.XaId).HasColumnName("XaID");

            entity.HasOne(d => d.DanToc).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DanTocId)
                .HasConstraintName("FK__Employee__DanToc__4316F928");

            entity.HasOne(d => d.Huyen).WithMany(p => p.Employees)
                .HasForeignKey(d => d.HuyenId)
                .HasConstraintName("FK__Employee__HuyenI__45F365D3");

            entity.HasOne(d => d.NgheNghiep).WithMany(p => p.Employees)
                .HasForeignKey(d => d.NgheNghiepId)
                .HasConstraintName("FK__Employee__NgheNg__440B1D61");

            entity.HasOne(d => d.Tinh).WithMany(p => p.Employees)
                .HasForeignKey(d => d.TinhId)
                .HasConstraintName("FK__Employee__TinhID__44FF419A");

            entity.HasOne(d => d.Xa).WithMany(p => p.Employees)
                .HasForeignKey(d => d.XaId)
                .HasConstraintName("FK__Employee__XaID__46E78A0C");
        });

        modelBuilder.Entity<NgheNghiep>(entity =>
        {
            entity.HasKey(e => e.NgheNghiepId).HasName("PK__NgheNghi__DDB652F9C1E3AC3D");

            entity.ToTable("NgheNghiep");

            entity.Property(e => e.NgheNghiepId).HasColumnName("NgheNghiepID");
            entity.Property(e => e.TenNgheNghiep).HasMaxLength(100);
        });

        modelBuilder.Entity<VanBang>(entity =>
        {
            entity.HasKey(e => e.VanBangId).HasName("PK__VanBang__BD0FB507AF0B5D07");

            entity.ToTable("VanBang", tb => tb.HasTrigger("trg_CheckMaxVanBang"));

            entity.Property(e => e.VanBangId).HasColumnName("VanBangID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.TenVanBang).HasMaxLength(100);

            entity.HasOne(d => d.DonViCapNavigation).WithMany(p => p.VanBangs)
                .HasForeignKey(d => d.DonViCap)
                .HasConstraintName("FK__VanBang__DonViCa__5CD6CB2B");

            entity.HasOne(d => d.Employee).WithMany(p => p.VanBangs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__VanBang__Employe__5DCAEF64");
        });

        
    }

    
}