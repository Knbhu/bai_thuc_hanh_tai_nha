using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PolicySearchService.Domain;

namespace PolicySearchService.DataAccess.Sql;

public sealed class SqlPolicyRepository : IPolicyRepository
{
    private readonly string connectionString;

    public SqlPolicyRepository(string connectionString)
    {
        this.connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentException("A SQL Server connection string is required.", nameof(connectionString));
    }

    public async Task Add(Policy policy)
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(@"
UPDATE pv
SET cover_from = @coverFrom,
    cover_to = @coverTo,
    total_premium = @totalPremium
FROM policy_versions pv
INNER JOIN policies p ON p.id = pv.policy_id
WHERE p.number = @number
  AND pv.version_number = (
      SELECT MAX(latest.version_number)
      FROM policy_versions latest
      WHERE latest.policy_id = p.id
  )", connection);

        command.Parameters.Add("@coverFrom", SqlDbType.DateTime2).Value = policy.PolicyStartDate.DateTime;
        command.Parameters.Add("@coverTo", SqlDbType.DateTime2).Value = policy.PolicyEndDate.DateTime;
        command.Parameters.Add("@totalPremium", SqlDbType.Decimal).Value = policy.PremiumAmount;
        command.Parameters["@totalPremium"].Precision = 18;
        command.Parameters["@totalPremium"].Scale = 2;
        command.Parameters.Add("@number", SqlDbType.NVarChar, 100).Value = policy.PolicyNumber;

        await connection.OpenAsync();
        if (await command.ExecuteNonQueryAsync() == 0)
            throw new InvalidOperationException(
                $"Policy '{policy.PolicyNumber}' was not found in SQL Server while synchronizing search data.");
    }

    public async Task<List<Policy>> Find(string queryText)
    {
        var terms = string.IsNullOrWhiteSpace(queryText)
            ? Array.Empty<string>()
            : queryText.Split(" AND ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var filters = terms.Select((_, index) =>
            $"(number LIKE @term{index} OR CONCAT(holder_first_name, ' ', holder_last_name) LIKE @term{index})");
        var whereClause = terms.Length == 0 ? string.Empty : $" AND {string.Join(" AND ", filters)}";
        var sql = @"
WITH CurrentVersions AS
(
    SELECT
        p.number,
        p.product_code,
        pv.cover_from,
        pv.cover_to,
        pv.holder_first_name,
        pv.holder_last_name,
        pv.total_premium,
        ROW_NUMBER() OVER (PARTITION BY p.id ORDER BY pv.version_number DESC) AS version_rank
    FROM policies p
    INNER JOIN policy_versions pv ON pv.policy_id = p.id
)
SELECT TOP (10) number, cover_from, cover_to, product_code,
       CONCAT(holder_first_name, ' ', holder_last_name) AS policy_holder, total_premium
FROM CurrentVersions
WHERE version_rank = 1" + whereClause + @"
ORDER BY cover_from DESC";

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        for (var index = 0; index < terms.Length; index++)
            command.Parameters.Add($"@term{index}", SqlDbType.NVarChar, 4000).Value = $"%{terms[index]}%";

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        var policies = new List<Policy>();
        while (await reader.ReadAsync())
        {
            policies.Add(new Policy
            {
                PolicyNumber = reader.GetString(0),
                PolicyStartDate = new DateTimeOffset(reader.GetDateTime(1)),
                PolicyEndDate = new DateTimeOffset(reader.GetDateTime(2)),
                ProductCode = reader.GetString(3),
                PolicyHolder = reader.GetString(4),
                PremiumAmount = reader.GetDecimal(5)
            });
        }

        return policies;
    }
}
