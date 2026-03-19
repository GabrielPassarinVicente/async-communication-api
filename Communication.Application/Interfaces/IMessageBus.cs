using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Application.Interfaces
{
    public interface IMessageBus
    {
        //public  um objeto qualquer na fila  
        Task PublishAsync<T>(T message, string queueName);
    }
}
