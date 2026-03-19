using Communication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Domain.Entities;

public class Scheduling
{
    public Guid Id { get; set; } //Guid (um identificador único universal)
    public DateTime ScheduleDate { get; private set; }
    public string Recipient { get; private set; }
    public string Message { get; private set; }
    public CommunicationType Type { get; private set; }
    public CommunicationStatus Status { get; private set; }

    // Construtor para inicializar o agendamento
    public Scheduling(DateTime scheduleDate, string recipient, string message, CommunicationType type)
    {
        Id = Guid.NewGuid();// gera um identificador único para cada agendamento
        ScheduleDate = scheduleDate;
        Recipient = recipient;
        Message = message;
        Type = type;
        Status = CommunicationStatus.Pending; // status inicial é "Pendente"
    }

    public void Cancel()
    {
        Status = CommunicationStatus.Cancelled;
    }
    public void MarkAsSent()
    {
        Status = CommunicationStatus.Sent;
    }
}

