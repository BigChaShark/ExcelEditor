using System;
using System.Collections.Generic;

namespace ExcelEditer.Models;

public partial class Loge
{
    public int Id { get; set; }

    public int LogeGroupId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int LogeSeqNo { get; set; }

    public int Status { get; set; }

    public int UpdateBy { get; set; }

    public DateTime LastUpdate { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int? IsOpen { get; set; }

    public int? IsRandom { get; set; }

    public int? IsConner { get; set; }

    public int? IsWalkWay { get; set; }

    public int? LogeOrder { get; set; }

    public int? LogeEvenOdd { get; set; }

    public virtual LogeGroup LogeGroup { get; set; } = null!;

    public virtual ICollection<LogeTempMaster> LogeTempMasters { get; set; } = new List<LogeTempMaster>();

    public virtual ICollection<LogeTempOffline> LogeTempOfflines { get; set; } = new List<LogeTempOffline>();
}
