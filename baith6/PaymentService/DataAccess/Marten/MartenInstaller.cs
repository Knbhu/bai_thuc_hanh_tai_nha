using Marten;
using Marten.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentService.Domain;
using Weasel.Core;

namespace PaymentService.DataAccess.Marten;

public static class MartenInstaller
{
    public static void AddMarten(this IServiceCollection services, string cnnString)
    {
        services.AddSingleton(CreateDocumentStore(cnnString));

        services.AddScoped<IDataStore, MartenDataStore>();
    }

    private static IDocumentStore CreateDocumentStore(string cn)
    {
        return DocumentStore.For(_ =>
        {
            _.Connection(cn);
            _.DatabaseSchemaName = "payment_service";
            _.UseNewtonsoftForSerialization(
                nonPublicMembersStorage: NonPublicMembersStorage.NonPublicSetters,
                configure: settings => settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects);
            _.Schema.For<PolicyAccount>()
                .Duplicate(t => t.PolicyNumber, "varchar(50)", configure: idx => idx.IsUnique = true);
        });
    }
}