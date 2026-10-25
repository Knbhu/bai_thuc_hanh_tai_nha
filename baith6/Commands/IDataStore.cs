using System.Collections.Generic;
using System.Threading.Tasks;
using PricingService.Domain;

namespace PricingService.Commands;

public interface IDataStore
{
    IDictionary<string, Task<Tariff>> Tariffs { get; }
}
