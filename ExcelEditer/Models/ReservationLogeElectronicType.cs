using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class ReservationLogeElectronicType
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public int? Price { get; set; }

    public int Status { get; set; }

    public int? Watt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
