using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Domain.Enums;

//a melhor prática é usar enum, que cria uma lista restrita de opções.
public enum CommunicationType
{
    Email,
    Sms,
    Push,
    WhatsApp
}

