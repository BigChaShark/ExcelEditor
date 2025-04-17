using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class Zone
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int Status { get; set; }

    public int UpdateBy { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ICollection<LogeGroup> LogeGroups { get; set; } = new List<LogeGroup>();

    public virtual ICollection<SubZone> SubZones { get; set; } = new List<SubZone>();
}
