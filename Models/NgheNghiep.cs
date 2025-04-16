using System;
using System.Collections.Generic;

namespace Tiennthe171977_Oceanteach.Models;

public partial class NgheNghiep
{
    public int NgheNghiepId { get; set; }

    public string TenNgheNghiep { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
