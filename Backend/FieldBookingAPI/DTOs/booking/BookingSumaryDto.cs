using System;
using System.Collections.Generic;


namespace FieldBookingAPI.DTOs
{
    public class BookingSummaryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public string Phone { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public string? HeroImage { get; set; }
        public string? Location { get; set; }
        public int TotalPrice { get; set; }
        public bool IsRead { get; set; }
        public string? ProcessStatus { get; set; }
        public string? PaymentImageUrl { get; set; }
        public string? StudentCardImageUrl { get; set; }
        public string Time { get; set; } = null!;
        public List<BookingSlotDto> Slots { get; set; } = new();

        // ⚡ Hàm static map từ Booking model sang DTO
        public static BookingSummaryDto FromModel(Models.Booking b)
        {
            return new BookingSummaryDto
            {
                Id = b.Id,
                Date = b.Date,
                CreatedAt = b.CreatedAt,
                Status = b.Status,
                Notes = b.Notes,
                Phone = b.Phone,
                UserName = b.UserName,
                FieldId = b.FieldId,
                FieldName = b.FieldName,
                HeroImage = b.Field?.HeroImage,
                Location = b.Field?.Location,
                TotalPrice = b.TotalPrice,
                IsRead = b.IsRead,
                ProcessStatus = b.ProcessStatus,
                PaymentImageUrl = b.PaymentImageUrl,
                StudentCardImageUrl = b.StudentCardImageUrl,
                Time = string.Join(", ", b.Slots.Select(s => s.Time)),
                Slots = b.Slots.Select(s => new BookingSlotDto
                {
                    SubField = s.SubField,
                    Time = s.Time
                }).ToList()
            };
        }
    }
}