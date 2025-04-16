using System;
using System.Collections.Generic;

namespace Tiennthe171977_Oceanteach.Models;

public partial class DanhMucXa
{
    public int XaId { get; set; }

    public string TenXa { get; set; } = null!;

    public int? HuyenId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual DanhMucHuyen? Huyen { get; set; }
}
