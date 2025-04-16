using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tiennthe171977_Oceanteach.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(Employee), nameof(ValidateNgaySinh))]
    public DateOnly? NgaySinh { get; set; }

    [Range(18, 100, ErrorMessage = "Tuổi phải từ 18 đến 100")]
    public int? Tuoi { get; set; }

    public int? DanTocId { get; set; }

    public int? NgheNghiepId { get; set; }

    [StringLength(12, MinimumLength = 12, ErrorMessage = "CCCD phải đúng 12 chữ số")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải là 12 chữ số")]
    public string? Cccd { get; set; }

    [RegularExpression(@"^0[3579]\d{8}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 03, 05, 07, 09 và có 10 chữ số")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 chữ số")]
    public string? SoDienThoai { get; set; }

    public int? TinhId { get; set; }

    public int? HuyenId { get; set; }

    public int? XaId { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ cụ thể không được vượt quá 200 ký tự")]
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

        // Nếu ngày sinh chưa đến trong năm hiện tại, giảm tuổi đi 1
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
