using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Application.Events
{
    public class CommunicationScheduledEvent
    {
        public Guid Id { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}
