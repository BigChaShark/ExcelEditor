using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class UserOffline
{
    public long Id { get; set; }

    public string UserOfflineId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public int Status { get; set; }

    public int ZoneId { get; set; }

    public int SubZoneId { get; set; }

    public int LogeQty { get; set; }

    public string LogeId { get; set; } = null!;

    public string LogeName { get; set; } = null!;

    public decimal LogeAmount { get; set; }

    public decimal ElectricityAmount { get; set; }

    public decimal ElectronicAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public DateOnly CreateDate { get; set; }

    public DateTime CreateDateTime { get; set; }

    public int CreateBy { get; set; }
}
