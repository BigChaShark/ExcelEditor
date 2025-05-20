using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class SubZone
{
    public int Id { get; set; }

    public int? ZoneId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int Status { get; set; }

    public int UpdateBy { get; set; }

    public DateTime LastUpdate { get; set; }

    public DateTime? Vipopen { get; set; }

    public DateTime? Vipclose { get; set; }

    public DateTime? GeneralOpen { get; set; }

    public DateTime? GeneralClose { get; set; }

    public int SaleGroup { get; set; }

    public virtual ICollection<LogeCostPerDay> LogeCostPerDays { get; set; } = new List<LogeCostPerDay>();

    public virtual ICollection<LogeGroup> LogeGroups { get; set; } = new List<LogeGroup>();

    public virtual ICollection<ReservationLoge> ReservationLoges { get; set; } = new List<ReservationLoge>();

    public virtual ICollection<Transaction> TransactionChangeReservationSubZones { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionReservationSubZones { get; set; } = new List<Transaction>();

    public virtual Zone? Zone { get; set; }
}
