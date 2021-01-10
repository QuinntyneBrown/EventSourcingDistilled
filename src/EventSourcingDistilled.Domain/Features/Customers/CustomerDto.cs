using System;

namespace EventSourcingDistilled.Domain.Features
{
    public class CustomerDto
    {        
        public Guid CustomerId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
