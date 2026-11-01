using System;
using MediatR;

namespace PolicyService.Api.Events;

public class PolicyTerminated : INotification
{
    public string PolicyNumber { get; set; } = string.Empty;
    public DateTime PolicyTo { get; set; }
    public decimal? AmountToReturn { get; set; }
}
