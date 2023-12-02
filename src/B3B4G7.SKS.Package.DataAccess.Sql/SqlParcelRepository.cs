using B3B4G7.SKS.Package.DataAccess.Entities;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces.Exceptions;
using B3B4G7.SKS.Package.DataAccess.Sql.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace B3B4G7.SKS.Package.DataAccess.Sql
{
    public class SqlParcelRepository : IParcelRepository
    {
        private TracknTraceContext _context;
        private ILogger<SqlParcelRepository> _logger;

        public SqlParcelRepository(TracknTraceContext context, ILogger<SqlParcelRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create(Parcel parcel, Point sender, Point recipient)
        {
            try
            {
                List<Warehouse> allWarehouses = _context.Warehouses.Include(x => x.NextHops).ThenInclude(x => x.Hop).ToList();
                if (allWarehouses.Count < 1)
                    throw new HopsNotExistInDbException("Warehouse hierarchy does not exist in the database.");

                List<Truck> allTrucks = _context.Trucks
                    .Include(x => x.WarehouseNextHops)
                    .ToList();

                _logger.LogInformation(sender.ToString());
                _logger.LogInformation(recipient.ToString());
                _logger.LogInformation(allTrucks.ToList().FirstOrDefault().Region.ToString());

                Truck senderHop = allTrucks.Where(t => t.Region.Covers(sender)).ToList().First();
                Truck recipientHop = allTrucks.Where(t => t.Region.Covers(recipient)).ToList().First();

                if (senderHop == null)
                    throw new HopsNotExistInDbException("Truck Hop for Sender does not exist in the database.");

                if (recipientHop == null)
                    throw new HopsNotExistInDbException("Truck Hop for Recipient does not exist in the database.");

                var tupleOfHopArrivalsAndTimes = PredictHops.CalculateRoute(senderHop, recipientHop);
                parcel.FutureHops = PredictHops.UpdateTimes(tupleOfHopArrivalsAndTimes);

                _context.Parcels.Add(parcel);
                _context.SaveChanges();
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

        public void UpdateState(Parcel parcel, Parcel.StateEnum state)
        {
            try
            {
                Parcel parcelUpdated = _context.Parcels.First(p => p.TrackingId == parcel.TrackingId);
                parcelUpdated.State = state;

                _context.SaveChanges();
                _logger.LogInformation($"Updated parcel state with data: {JsonConvert.SerializeObject(parcelUpdated, Formatting.Indented)}");
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(InvalidOperationException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(message);
            }
        }

        public void UpdateListHops(Parcel parcel, string code)
        {
            try
            {
                Parcel parcelToUpdate = _context.Parcels.First(p => p.TrackingId == parcel.TrackingId);

                List<HopArrival> hopArrivals = _context.HopArrivals.Where(ha => ha.FutureHopsFK == parcel.TrackingId).ToList();
                int index = hopArrivals.FindIndex(a => a.Code == code);

                parcelToUpdate.FutureHops.RemoveRange(0, index+1);
                var subArray = hopArrivals.Skip(0).Take(index+1).ToList();

                parcelToUpdate.VisitedHops.AddRange(subArray);

                _context.SaveChanges();
                _logger.LogInformation($"Updated parcel future hops and visited hops with data: {JsonConvert.SerializeObject(parcelToUpdate, Formatting.Indented)}");
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(InvalidOperationException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(message);
            }
        }

        public void UpdateAllListHops(Parcel parcel)
        {
            try
            {
                Parcel parcelToUpdate = _context.Parcels.First(p => p.TrackingId == parcel.TrackingId);

                List<HopArrival> hopArrivals = _context.HopArrivals.Where(ha => ha.FutureHopsFK == parcel.TrackingId).ToList();

                parcelToUpdate.FutureHops.RemoveRange(0, hopArrivals.Count);

                parcelToUpdate.VisitedHops.AddRange(hopArrivals);

                _context.SaveChanges();
                _logger.LogInformation($"Updated parcel future hops and visited hops with data: {JsonConvert.SerializeObject(parcelToUpdate, Formatting.Indented)}");
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(InvalidOperationException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(message);
            }
        }

        public Parcel GetByTrackingId(string trackingId)
        {
            try
            {
                Parcel parcel = _context.Parcels
                    .Include(p => p.Sender)
                    .Include(p => p.Recipient)
                    .Include(p => p.VisitedHops)
                    .Include(p => p.FutureHops)
                    .AsSplitQuery()
                    .ToList()
                    .FirstOrDefault(x => x.TrackingId == trackingId);

                _logger.LogInformation($"Found parcel data with: {JsonConvert.SerializeObject(parcel, Formatting.Indented)}");
                return parcel;
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(InvalidOperationException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(message);
            }

            /*return new Parcel
            {
                State = Parcel.StateEnum.PickupEnum,
                VisitedHops = new List<HopArrival>()
                {
                    new() { Code = "code1", Description = "description1", DateTime = DateTime.Now },
                    new() { Code = "code2", Description = "description2", DateTime = DateTime.Now },
                },
                FutureHops = new List<HopArrival>()
                {
                    new() { Code = "code3", Description = "description3", DateTime = DateTime.Now },
                    new() { Code = "code4", Description = "description4", DateTime = DateTime.Now },
                }
            };*/
        }
    }
}