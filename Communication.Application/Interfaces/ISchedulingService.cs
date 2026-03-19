using Communication.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Application.Interfaces;

public interface ISchedulingService
{
    Task<Guid> ScheduleAsync(ScheduleRequestDto request);
    Task<string?> GetStatusAsync(Guid id);

    
    Task CancelAsync(Guid id);
}

