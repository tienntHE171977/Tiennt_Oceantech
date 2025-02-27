using System;
using System.Collections.Generic;

namespace Tiennthe171977_Oceanteach.Models;

public partial class DanToc
{
    public int DanTocId { get; set; }

    public string TenDanToc { get; set; } = null!;

    

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
