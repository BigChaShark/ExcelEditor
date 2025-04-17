using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

public partial class LogeTempMaster
{
    public int LogeId { get; set; }

    public string LogeName { get; set; } = null!;

    public int LogeTypeId { get; set; }

    public int LogeIndex { get; set; }

    public int IsConner { get; set; }

    /// <summary>
    /// 1 = จันทร์ - พฤหัส , 2 = ศุกร์ - อาทิตย์
    /// </summary>
    public int OpenCase { get; set; }

    public int Status { get; set; }

    public virtual Loge Loge { get; set; } = null!;
}
