using Communication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Application.DTOs;

public class ScheduleRequestDto
{
    public DateTime ScheduleDate { get; set; }
    public  string Recipient { get; set; }
    public string Message { get; set; }
    public CommunicationType Type { get; set; }
}

