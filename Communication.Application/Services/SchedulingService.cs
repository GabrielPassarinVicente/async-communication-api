using Communication.Application.DTOs;
using Communication.Application.Events;
using Communication.Application.Interfaces;
using Communication.Domain.Entities;
using Communication.Domain.Interfaces;

namespace Communication.Application.Services;

public class SchedulingService : ISchedulingService
{
    private readonly ISchedulingRepository _schedulingRepository;
    private readonly IMessageBus _messageBus;

    public SchedulingService(ISchedulingRepository schedulingRepository, IMessageBus messageBus)
    {
        _schedulingRepository = schedulingRepository;
        _messageBus = messageBus;
    }

    public async Task<Guid> ScheduleAsync(ScheduleRequestDto request)
    {
        var scheduling = new Scheduling(request.ScheduleDate, request.Recipient, request.Message, request.Type);

        await _schedulingRepository.AddAsync(scheduling);

        var eventMessage = new CommunicationScheduledEvent
        {
            Id = scheduling.Id,
            Recipient = scheduling.Recipient,
            Message = scheduling.Message,
            Type = scheduling.Type.ToString()
        };

        await _messageBus.PublishAsync(eventMessage, "communication-requests");
        return scheduling.Id;
    }

    public async Task<string?> GetStatusAsync(Guid id)
    {
        var scheduling = await _schedulingRepository.GetByIdAsync(id);
        return scheduling?.Status.ToString();
    }

    public async Task CancelAsync(Guid id)
    {
        var scheduling = await _schedulingRepository.GetByIdAsync(id);
        if (scheduling is null)
        {
            return;
        }

        scheduling.Cancel();
        await _schedulingRepository.UpdateAsync(scheduling);
    }
}