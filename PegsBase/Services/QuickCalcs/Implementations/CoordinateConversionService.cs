using Npgsql;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.QuickCalcs;
using PegsBase.Services.QuickCalcs.Interfaces;

namespace PegsBase.Services.QuickCalcs.Implementations
{
    public class CoordinateConversionService : ICoordinateConversionService
    {
        private readonly ApplicationDbContext _dbContext;
        public CoordinateConversionService(ApplicationDbContext ctx)
            => _dbContext = ctx;

        public async Task<CoordinateConversionResult> ConvertAsync(
            double x, double y, int sourceSrid, int targetSrid)
        {
            var sql = @"
              SELECT 
                ST_X(pt) AS X, 
                ST_Y(pt) AS Y,
                ST_Z(pt) AS Z
              FROM (
                SELECT ST_Transform(
                         ST_SetSRID(
                           ST_MakePoint(@x, @y), @src
                         ),
                         @dst
                       ) AS pt
              ) sub;";

            await _dbContext.Database.OpenConnectionAsync();
            using var cmd = _dbContext.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("x", x));
            cmd.Parameters.Add(new NpgsqlParameter("y", y));
            cmd.Parameters.Add(new NpgsqlParameter("src", sourceSrid));
            cmd.Parameters.Add(new NpgsqlParameter("dst", targetSrid));

            using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync())
                throw new InvalidOperationException("Conversion failed.");

            return new CoordinateConversionResult
            {
                X = r.GetDouble(0),
                Y = r.GetDouble(1),
                Z = r.IsDBNull(2) ? (double?)null : r.GetDouble(2)
            };
        }
    }
}
