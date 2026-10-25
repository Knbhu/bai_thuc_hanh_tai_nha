using MediatR;

namespace PolicyService.Api.Events;

public class PolicyCreated : INotification
{
    public string PolicyNumber { get; set; } = string.Empty;
    public DateTime PolicyFrom { get; set; }
    public DateTime PolicyTo { get; set; }
    public PolicyHolder PolicyHolder { get; set; } = new();
    public string ProductCode { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public string AgentLogin { get; set; } = string.Empty;
}

public class PolicyHolder
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
