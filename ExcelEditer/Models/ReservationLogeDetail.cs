using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class ReservationLogeDetail
{
    public long? ReservationLogeId { get; set; }

    public int LogeId { get; set; }

    public long ReservationDate { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Remark { get; set; }

    public long? EstampId { get; set; }

    public virtual Loge Loge { get; set; } = null!;

    public virtual ReservationLoge? ReservationLoge { get; set; }
}
