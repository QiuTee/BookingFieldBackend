using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FieldBookingAPI.DTOs
{
    public class BookingDto
    {
        public string FieldName { get; set; } = null!;
        public string BookingCode { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public List<BookingSlotDto> Slots { get; set; } = new();

        public string UserName { get; set; } = null!;
        public int TotalPrice { get; set; } = 0;
        public string Phone { get; set; } = null!;

        public string? Notes { get; set; }
        public int FieldId { get; set; }
    }
}
