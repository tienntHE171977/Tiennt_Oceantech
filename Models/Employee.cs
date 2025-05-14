using System.ComponentModel.DataAnnotations;

namespace Tiennthe171977_Oceanteach.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    
    public string HoTen { get; set; } = null!;

    
    public DateOnly? NgaySinh { get; set; }

    
    public int? Tuoi { get; set; }

    public int? DanTocId { get; set; }

    public int? NgheNghiepId { get; set; }

    
    public string? Cccd { get; set; }

    
    public string? SoDienThoai { get; set; }

    public int? TinhId { get; set; }

    public int? HuyenId { get; set; }

    public int? XaId { get; set; }

    
    public string? DiaChiCuThe { get; set; }

    public virtual DanToc? DanToc { get; set; }

    public virtual DanhMucHuyen? Huyen { get; set; }

    public virtual NgheNghiep? NgheNghiep { get; set; }

    public virtual DanhMucTinh? Tinh { get; set; }

    public virtual List<VanBang> VanBangs { get; set; } = new List<VanBang>();

    public virtual DanhMucXa? Xa { get; set; }
    

    public static ValidationResult? ValidateNgaySinh(DateOnly? ngaySinh, ValidationContext context)
    {
        if (!ngaySinh.HasValue)
        {
            return new ValidationResult("Ngày sinh là bắt buộc.");
        }

        var employee = (Employee)context.ObjectInstance;
        int calculatedAge = DateTime.Now.Year - ngaySinh.Value.Year;

        
        if (ngaySinh.Value > DateOnly.FromDateTime(DateTime.Now.AddYears(-calculatedAge)))
        {
            calculatedAge--;
        }

        if (employee.Tuoi.HasValue && employee.Tuoi != calculatedAge)
        {
            return new ValidationResult("Tuổi không khớp với ngày sinh.");
        }

        return ValidationResult.Success;
    }

    public int ValidVanBangCount => VanBangs.Count(vb => !vb.NgayHetHan.HasValue || vb.NgayHetHan > DateOnly.FromDateTime(DateTime.Now));
    public int TotalVanBangCount => VanBangs.Count;
}