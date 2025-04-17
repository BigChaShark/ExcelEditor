using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class LogeTempOffline
{
    public decimal Seq { get; set; }

    public int LogeId { get; set; }

    public string LogeName { get; set; } = null!;

    public DateOnly OpenDateInt { get; set; }

    public int LogeTypeId { get; set; }

    public int LogeIndex { get; set; }

    public int IsConner { get; set; }

    public int Status { get; set; }

    public virtual Loge Loge { get; set; } = null!;
}
