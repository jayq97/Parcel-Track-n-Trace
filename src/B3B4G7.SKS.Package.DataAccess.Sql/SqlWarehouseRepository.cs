using B3B4G7.SKS.Package.DataAccess.Entities;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.DataAccess.Interfaces.Exceptions;

namespace B3B4G7.SKS.Package.DataAccess.Sql
{
    public class SqlWarehouseRepository : IWarehouseRepository
    {
        private TracknTraceContext _context;
        private ILogger<SqlWarehouseRepository> _logger;

        public SqlWarehouseRepository(TracknTraceContext context, ILogger<SqlWarehouseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Warehouse Create(Warehouse warehouse)
        {
            try
            {
                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
                _logger.LogInformation("Warehouse created.");
                return warehouse;
            }
            catch (OperationCanceledException ex)
            {
                string message = nameof(OperationCanceledException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new OperationCanceledException(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                string message = nameof(DbUpdateException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new DbUpdateException(ex.Message);
            }
        }

        public Warehouse GetWarehouses()
        {
            try
            {
                var wh = _context.Warehouses.Where(a => a.Level == 0)
                        .Include(a => a.NextHops)
                        .ThenInclude(a => a.Hop);

                if(wh.ToList().Count < 0)
                    throw new HopsNotExistInDbException("Warehouse hierarchy does not exist in the database.");

                int maxLevel = _context.Warehouses.ToList().Max(p => p.Level);

                for (var i = 0; i < maxLevel; i++)
                {
                    wh = wh.ThenInclude(a => (a as Warehouse).NextHops)
                        .ThenInclude(a => a.Hop);
                }
                var warehouseHierarchy = wh
                        .AsSplitQuery()
                        .ToList()
                        .FirstOrDefault();

                return warehouseHierarchy;
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(InvalidOperationException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }

        public Hop GetByCode(string code)
        {
            try
            {
                Hop hop = _context.Hops.FirstOrDefault(w => w.Code == code);

                if(hop.HopType == "warehouse")
                {
                    var wareHouse = _context.Warehouses.Where(w => w.Code == code);

                    var wareHouseHierarchy = wareHouse
                        .Include(a => a.NextHops)
                        .ThenInclude(a => a.Hop);

                    for (var i = 0; i < wareHouse.FirstOrDefault().Level; i++)
                    {
                        wareHouseHierarchy = wareHouseHierarchy
                            .ThenInclude(a => (a as Warehouse).NextHops)
                            .ThenInclude(a => a.Hop);
                    }

                    return wareHouseHierarchy
                            .AsSplitQuery()
                            .ToList()
                            .FirstOrDefault();
                }

                else if(hop.HopType == "truck")
                    hop ??= _context.Trucks.FirstOrDefault(tr => tr.Code == code);

                else
                    hop ??= _context.Transferwarehouses.FirstOrDefault(tw => tw.Code == code);

                return hop;
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(InvalidOperationException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }

        public void ClearDatabase()
        {
            try
            {
                /*var tableNames = _context.Model.GetEntityTypes()
                    .Select(t => t.GetTableName())
                    .Distinct()
                    .ToList();

                tableNames.Reverse();

                foreach (var tableName in tableNames)
                {
                    _logger.LogInformation(tableName);
                    _context.Database.ExecuteSqlRaw($"DELETE FROM {tableName}");
                }*/

                _context.Database.ExecuteSqlRaw($"DELETE FROM WebhookResponses");
                _context.Database.ExecuteSqlRaw($"DELETE FROM WarehouseNextHops");
                _context.Database.ExecuteSqlRaw($"DELETE FROM Warehouses");
                _context.Database.ExecuteSqlRaw($"DELETE FROM Trucks");
                _context.Database.ExecuteSqlRaw($"DELETE FROM TransferWarehouses");
                _context.Database.ExecuteSqlRaw($"DELETE FROM HopArrivals");
                _context.Database.ExecuteSqlRaw($"DELETE FROM Parcels");
                _context.Database.ExecuteSqlRaw($"DELETE FROM Persons");
                _context.Database.ExecuteSqlRaw($"DELETE FROM Hops");

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }
    }
}
