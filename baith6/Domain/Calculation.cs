using System;
using System.Collections.Generic;

namespace PricingService.Domain;

public class Calculation
{
    public Calculation(
        string productCode,
        DateTimeOffset policyFrom,
        DateTimeOffset policyTo,
        IEnumerable<string> selectedCovers,
        IDictionary<string, object> subject)
    {
        ProductCode = productCode;
        PolicyFrom = policyFrom;
        PolicyTo = policyTo;
        SelectedCovers = selectedCovers;
        Subject = subject;
    }

    public string ProductCode { get; }
    public DateTimeOffset PolicyFrom { get; }
    public DateTimeOffset PolicyTo { get; }
    public IEnumerable<string> SelectedCovers { get; }
    public IDictionary<string, object> Subject { get; }
}
