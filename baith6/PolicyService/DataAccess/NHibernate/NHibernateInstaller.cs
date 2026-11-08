using System.Data;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using PolicyService.Domain;

namespace PolicyService.DataAccess.NHibernate;

public static class NHibernateInstaller
{
    public static IServiceCollection AddNHibernate(
        this IServiceCollection services,
        string databaseProvider,
        string cnString)
    {
        var cfg = new Configuration();

        cfg.DataBaseIntegration(db =>
        {
            switch (databaseProvider)
            {
                case "SqlServer":
                    db.Dialect<MsSql2012Dialect>();
                    db.Driver<MicrosoftDataSqlClientDriver>();
                    break;
                case "PostgreSQL":
                    db.Dialect<PostgreSQL83Dialect>();
                    db.Driver<NpgsqlDriver>();
                    break;
                default:
                    throw new System.InvalidOperationException(
                        $"Unsupported database provider '{databaseProvider}'.");
            }

            db.ConnectionProvider<DriverConnectionProvider>();
            db.BatchSize = 500;
            db.IsolationLevel = IsolationLevel.ReadCommitted;
            db.LogSqlInConsole = false;
            db.ConnectionString = cnString;
            db.Timeout = 30; /*seconds*/
            db.SchemaAction = SchemaAutoAction.Update;
        });

        cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());

        cfg.Cache(c => c.UseQueryCache = false);

        cfg.AddAssembly(typeof(NHibernateInstaller).Assembly);

        services.AddSingleton(cfg.BuildSessionFactory());

        services.AddScoped(s => s.GetService<ISessionFactory>().OpenSession());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}