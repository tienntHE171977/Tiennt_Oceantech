using System;
using System.Collections.Generic;

namespace Tiennthe171977_Oceanteach.Models;

public partial class DanhMucHuyen
{
    public int HuyenId { get; set; }

    public string TenHuyen { get; set; } = null!;

    public int? TinhId { get; set; }

    

    public virtual ICollection<DanhMucXa> DanhMucXas { get; set; } = new List<DanhMucXa>();

    

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual DanhMucTinh? Tinh { get; set; }
}
