using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class LogeCostPerDay
{
    public int SubZoneId { get; set; }

    public string Day { get; set; } = null!;

    public int Seq { get; set; }

    public decimal Cost { get; set; }

    public decimal? Cost2 { get; set; }

    public virtual SubZone SubZone { get; set; } = null!;
}
