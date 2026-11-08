using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DashboardService.Domain;
using Microsoft.Data.SqlClient;

namespace DashboardService.DataAccess.Sql;

public sealed class SqlPolicyRepository : IPolicyRepository
{
    private const string CurrentPoliciesSql = @"
WITH CurrentVersions AS
(
    SELECT
        p.number,
        p.product_code,
        p.agent_login,
        pv.cover_from,
        pv.cover_to,
        pv.holder_first_name,
        pv.holder_last_name,
        pv.total_premium,
        ROW_NUMBER() OVER (PARTITION BY p.id ORDER BY pv.version_number DESC) AS version_rank
    FROM policies p
    INNER JOIN policy_versions pv ON pv.policy_id = p.id
)
SELECT number, cover_from, cover_to, holder_first_name, holder_last_name,
       product_code, total_premium, agent_login
FROM CurrentVersions
WHERE version_rank = 1";

    private readonly string connectionString;

    public SqlPolicyRepository(string connectionString)
    {
        this.connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentException("A SQL Server connection string is required.", nameof(connectionString));
    }

    public void Save(PolicyDocument policy)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        using (var policyCommand = new SqlCommand(
                   "UPDATE policies SET product_code = @productCode, agent_login = @agentLogin WHERE number = @number",
                   connection,
                   transaction))
        {
            policyCommand.Parameters.Add("@productCode", SqlDbType.NVarChar, 100).Value = policy.ProductCode;
            policyCommand.Parameters.Add("@agentLogin", SqlDbType.NVarChar, 200).Value = policy.AgentLogin ?? string.Empty;
            policyCommand.Parameters.Add("@number", SqlDbType.NVarChar, 100).Value = policy.Number;

            if (policyCommand.ExecuteNonQuery() == 0)
                throw new InvalidOperationException($"Policy '{policy.Number}' was not found in SQL Server.");
        }

        var holder = policy.PolicyHolder?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        using (var versionCommand = new SqlCommand(@"
UPDATE pv
SET cover_from = @coverFrom,
    cover_to = @coverTo,
    holder_first_name = @firstName,
    holder_last_name = @lastName,
    total_premium = @totalPremium
FROM policy_versions pv
INNER JOIN policies p ON p.id = pv.policy_id
WHERE p.number = @number
  AND pv.version_number = (
      SELECT MAX(latest.version_number)
      FROM policy_versions latest
      WHERE latest.policy_id = p.id
  )", connection, transaction))
        {
            versionCommand.Parameters.Add("@coverFrom", SqlDbType.DateTime2).Value = policy.From;
            versionCommand.Parameters.Add("@coverTo", SqlDbType.DateTime2).Value = policy.To;
            versionCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 200).Value =
                holder.Length > 0 ? holder[0] : string.Empty;
            versionCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 200).Value =
                holder.Length > 1 ? holder[1] : string.Empty;
            versionCommand.Parameters.Add("@totalPremium", SqlDbType.Decimal).Value = policy.TotalPremium;
            versionCommand.Parameters["@totalPremium"].Precision = 18;
            versionCommand.Parameters["@totalPremium"].Scale = 2;
            versionCommand.Parameters.Add("@number", SqlDbType.NVarChar, 100).Value = policy.Number;

            if (versionCommand.ExecuteNonQuery() == 0)
                throw new InvalidOperationException($"Policy '{policy.Number}' has no version in SQL Server.");
        }

        transaction.Commit();
    }

    public PolicyDocument FindByNumber(string policyNumber)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(CurrentPoliciesSql + " AND number = @number", connection);
        command.Parameters.Add("@number", SqlDbType.NVarChar, 100).Value = policyNumber;
        connection.Open();

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadPolicy(reader) : null;
    }

    public AgentSalesQueryResult GetAgentSales(AgentSalesQuery query)
    {
        var policies = ReadPolicies(
            query.FilterBySalesDateStart,
            query.FilterBySalesDateEnd,
            query.FilterByProductCode,
            query.FilterByAgentLogin);

        var result = new AgentSalesQueryResult();
        foreach (var group in policies.GroupBy(policy => policy.AgentLogin ?? string.Empty))
        {
            result.AgentResult(group.Key, new SalesResult(
                group.LongCount(),
                group.Sum(policy => policy.TotalPremium)));
        }

        return result;
    }

    public TotalSalesQueryResult GetTotalSales(TotalSalesQuery query)
    {
        var policies = ReadPolicies(
            query.FilterBySalesDateStart,
            query.FilterBySalesDateEnd,
            query.FilterByProductCode);

        var result = new TotalSalesQueryResult();
        foreach (var group in policies.GroupBy(policy => policy.ProductCode ?? string.Empty))
        {
            result.ProductResult(group.Key, new SalesResult(
                group.LongCount(),
                group.Sum(policy => policy.TotalPremium)));
        }

        return result;
    }

    public SalesTrendsResult GetSalesTrend(SalesTrendsQuery query)
    {
        var policies = ReadPolicies(
            query.FilterBySalesDateStart,
            query.FilterBySalesDateEnd,
            query.FilterByProductCode);
        var result = new SalesTrendsResult();

        foreach (var group in policies
                     .GroupBy(policy => StartOfPeriod(policy.From, query.AggregationUnit))
                     .OrderBy(group => group.Key))
        {
            result.PeriodResult(new PeriodSales(
                group.Key,
                group.Key.ToShortDateString(),
                new SalesResult(group.LongCount(), group.Sum(policy => policy.TotalPremium))));
        }

        return result;
    }

    private List<PolicyDocument> ReadPolicies(
        DateTime from,
        DateTime to,
        string productCode,
        string agentLogin = null)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(CurrentPoliciesSql + @"
 AND cover_from >= @fromDate
 AND cover_from <= @toDate
 AND (@productCode IS NULL OR product_code = @productCode)
 AND (@agentLogin IS NULL OR agent_login = @agentLogin)", connection);

        command.Parameters.Add("@fromDate", SqlDbType.DateTime2).Value = from;
        command.Parameters.Add("@toDate", SqlDbType.DateTime2).Value = to;
        command.Parameters.Add("@productCode", SqlDbType.NVarChar, 100).Value =
            (object)productCode ?? DBNull.Value;
        command.Parameters.Add("@agentLogin", SqlDbType.NVarChar, 200).Value =
            (object)agentLogin ?? DBNull.Value;
        connection.Open();

        using var reader = command.ExecuteReader();
        var policies = new List<PolicyDocument>();
        while (reader.Read())
            policies.Add(ReadPolicy(reader));

        return policies;
    }

    private static PolicyDocument ReadPolicy(SqlDataReader reader)
    {
        var from = reader.GetDateTime(1);
        var to = reader.IsDBNull(2) ? from : reader.GetDateTime(2);
        var holder = string.Join(" ", new[] { reader.GetString(3), reader.GetString(4) }.Where(name => name.Length > 0));
        return new PolicyDocument(
            reader.GetString(0),
            from,
            to,
            holder,
            reader.GetString(5),
            reader.GetDecimal(6),
            reader.IsDBNull(7) ? string.Empty : reader.GetString(7));
    }

    private static DateTime StartOfPeriod(DateTime date, TimeAggregationUnit unit)
    {
        var day = date.Date;
        return unit switch
        {
            TimeAggregationUnit.Day => day,
            TimeAggregationUnit.Week => day.AddDays(-(((int)day.DayOfWeek + 6) % 7)),
            TimeAggregationUnit.Month => new DateTime(day.Year, day.Month, 1),
            TimeAggregationUnit.Year => new DateTime(day.Year, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported sales aggregation unit.")
        };
    }
}
