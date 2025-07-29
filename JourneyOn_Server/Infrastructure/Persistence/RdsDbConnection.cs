using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

// using Amazon.RDS.Util;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace Infrastructure.Persistence;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public static class RdsDbConnection
{
    public static async Task<bool> ConnectToDbAsync(IConfiguration configuration, string connectionString)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            await conn.CloseAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}