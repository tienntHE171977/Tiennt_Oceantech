using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tiennthe171977_Oceanteach.Models;

public partial class VanBang
{
    [Key]
    public int VanBangId { get; set; }

    [Required(ErrorMessage = "Tên văn bằng là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên văn bằng không được vượt quá 100 ký tự.")]
    public string TenVanBang { get; set; } = null!;

    [Required(ErrorMessage = "Ngày cấp là bắt buộc.")]
    public DateOnly NgayCap { get; set; }

    public int? DonViCap { get; set; }

   
    public DateOnly? NgayHetHan { get; set; }

    public int? EmployeeId { get; set; }

    public virtual DanhMucTinh? DonViCapNavigation { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; }

    
}