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

    public virtual DanhMucXa? Xa { get; set; }

    // Phương thức validation tùy chỉnh cho Ngày sinh
    public static ValidationResult? ValidateNgaySinh(DateOnly? ngaySinh, ValidationContext context)
    {
        if (ngaySinh.HasValue)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (ngaySinh.Value > today)
            {
                return new ValidationResult("Ngày sinh không được là ngày trong tương lai.");
            }

            // Tính tuổi từ ngày sinh để kiểm tra tính hợp lệ với Tuoi
            int calculatedAge = today.Year - ngaySinh.Value.Year;
            if (ngaySinh.Value > today.AddYears(-calculatedAge)) calculatedAge--;

            var employee = (Employee)context.ObjectInstance;
            if (employee.Tuoi.HasValue && employee.Tuoi != calculatedAge)
            {
                return new ValidationResult("Tuổi không khớp với ngày sinh.");
            }
        }
        return ValidationResult.Success;
    }
}