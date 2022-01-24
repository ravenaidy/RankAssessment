using MediatR;
using System;

namespace RES.ATM.API.Shared
{
    public class BaseEvent : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
