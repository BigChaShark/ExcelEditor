using System;
using System.Collections.Generic;

namespace ExcelEditor.Models;

/// <summary>
/// RPI = Request Payment Input
/// RPO = Request Payment Output
/// PRN = Payment result for HTTP post parameter
/// PRS =  Payment result for respUrl (Silent Post)
/// </summary>
public partial class Transaction
{
    public long TranId { get; set; }

    public int TransactionStatusId { get; set; }

    public string TranCode { get; set; } = null!;

    public long MemberId { get; set; }

    public int PaymentGatewayId { get; set; }

    public string? PaymentEndPointUrl { get; set; }

    public long? ReservationLogeId { get; set; }

    public decimal AmountToPay { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? ReservationsDate { get; set; }

    public DateTime? CancleDate { get; set; }

    public string? PaymentReferenceCode { get; set; }

    public string? PaymentResponseCode { get; set; }

    public string? PaymentResponseMessage { get; set; }

    public int? PaymentTypeId { get; set; }

    public int? RcptType { get; set; }

    public string? RcptNo { get; set; }

    public int? ReservationLogeTypeId { get; set; }

    public int? ReservationLogeElectricityTypeId { get; set; }

    public int? ReservationLogeElectronicTypeId { get; set; }

    public int? ReservationSubZoneId { get; set; }

    public int? StaffUserId { get; set; }

    public int? TransactionTypeId { get; set; }

    public int? TransactionSubZoneId { get; set; }

    public int? BypassPaymentStatusId { get; set; }

    public int? IsAddLogeAuto { get; set; }

    public int? ChangeReservationSubZoneId { get; set; }

    public string? ReservationsResult { get; set; }

    public DateTime? ReservationsRound { get; set; }

    public int? CancelStaffUserId { get; set; }

    public int? ChangeLogeStaffUserId { get; set; }

    public int? ChangeElecStaffUserId { get; set; }

    public int? IsHelpDesk { get; set; }

    public string? IsBillVat { get; set; }

    public int? LogeId { get; set; }

    /// <summary>
    /// null = ไม่มีส่วนลด | 1 = บันทึกส่วนลดฝนตก | 2 = บันทึกส่วนลดฝนตก (free day)
    /// </summary>
    public int? IsDiscountRain { get; set; }

    /// <summary>
    /// null = ไม่มีส่วนลด | 1 = บันทึกส่วนลดฝนตก | 2 = บันทึกส่วนลดฝนตก (free day)
    /// </summary>
    public int? IsDiscountRainFreeDay { get; set; }

    public long? IsRefundMuSaving { get; set; }

    public long? IsRefundWallet { get; set; }

    public int? IsGuest { get; set; }

    public string? RcptCode { get; set; }

    public int? IsChangeZone { get; set; }

    public string? RcptCode2 { get; set; }

    public virtual SubZone? ChangeReservationSubZone { get; set; }

    public virtual Loge? Loge { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ReservationLoge? ReservationLoge { get; set; }

    public virtual ReservationLogeElectricityType? ReservationLogeElectricityType { get; set; }

    public virtual ReservationLogeElectronicType? ReservationLogeElectronicType { get; set; }

    public virtual SubZone? ReservationSubZone { get; set; }
}
