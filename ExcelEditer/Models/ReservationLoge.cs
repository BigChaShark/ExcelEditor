using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class ReservationLoge
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public int? ZoneId { get; set; }

    public int LogeQty { get; set; }

    public string LogeName { get; set; } = null!;

    public decimal LogeAmount { get; set; }

    public decimal ElectricityAmount { get; set; }

    public decimal ElectronicAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal FullAreaAmount { get; set; }

    public decimal FineAmount { get; set; }

    public int Status { get; set; }

    public DateTime CreateDate { get; set; }

    public int? SubZoneId { get; set; }

    public int? IsEntrance { get; set; }

    public int? ReservationStatus { get; set; }

    public decimal? SavingAmount { get; set; }

    public decimal? PaymentFee { get; set; }

    public decimal? EstampAmount { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<ReservationLogeDetail> ReservationLogeDetails { get; set; } = new List<ReservationLogeDetail>();

    public virtual SubZone? SubZone { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual Zone? Zone { get; set; }
}
