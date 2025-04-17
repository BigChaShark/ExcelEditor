using System;
using System.Collections.Generic;

namespace ExcelEditer.Models;

public partial class LogeGroup
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int GroupSeqNo { get; set; }

    public int ZoneId { get; set; }

    public int Status { get; set; }

    public int UpdateBy { get; set; }

    public DateTime LastUpdate { get; set; }

    public int? SubZoneId { get; set; }

    public virtual ICollection<Loge> Loges { get; set; } = new List<Loge>();

    public virtual SubZone? SubZone { get; set; }

    public virtual Zone Zone { get; set; } = null!;
}
