using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class UserOffline
{
    public long Id { get; set; }

    public string? UserOfflineId { get; set; }

    public string? Name { get; set; }

    public string? Mobile { get; set; }

    public int? Status { get; set; }

    public int? ZoneId { get; set; }

    public int? SubZoneId { get; set; }

    public int? LogeQty { get; set; }

    public string? LogeId { get; set; }

    public string? LogeName { get; set; }

    public DateOnly? CreateDate { get; set; }

    public DateTime? CreateDateTime { get; set; }

    public int? CreateBy { get; set; }
}
