using B3B4G7.SKS.Package.DataAccess.Entities;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3B4G7.SKS.Package.DataAccess.Sql.Utils
{
    static class PredictHops
    {
        internal static Tuple<List<HopArrival>, List<int>> CalculateRoute(Hop senderHop, Hop recipientHop)
        {

            if (senderHop.Code == recipientHop.Code)
            {
                var commonParent = senderHop;
                Console.WriteLine(commonParent.ProcessingDelayMins.ToString());
                return Tuple.Create(new List<HopArrival>() { new HopArrival()
                {
                    Code = commonParent.Code,
                    Description = commonParent.Description,
                } }, new List<int>() { commonParent.ProcessingDelayMins });
            }

            else
            {
                var route = CalculateRoute(senderHop.WarehouseNextHops.Parent, recipientHop.WarehouseNextHops.Parent);

                var senderParentHopArrival = new HopArrival()
                {
                    Code = senderHop.Code,
                    Description = senderHop.Description,
                };

                var recipientParentHopArrival = new HopArrival()
                {
                    Code = recipientHop.Code,
                    Description = recipientHop.Description,
                };

                route.Item1.Insert(0, senderParentHopArrival);
                Console.WriteLine(senderHop.ProcessingDelayMins.ToString() + " " + senderHop.WarehouseNextHops.TraveltimeMins.ToString());
                route.Item2.Insert(0, senderHop.ProcessingDelayMins + senderHop.WarehouseNextHops.TraveltimeMins);

                route.Item1.Add(recipientParentHopArrival);
                Console.WriteLine(recipientHop.ProcessingDelayMins.ToString() + " " + recipientHop.WarehouseNextHops.TraveltimeMins.ToString());
                route.Item2.Add(recipientHop.ProcessingDelayMins + recipientHop.WarehouseNextHops.TraveltimeMins);

                return route;
            }
        }

        internal static List<HopArrival> UpdateTimes(Tuple<List<HopArrival>, List<int>> tupleOfHopArrivalsAndTimes)
        {
            var dateTime = DateTime.Now;

            for (var i = 0; i < tupleOfHopArrivalsAndTimes.Item1.Count; i++)
            {
                dateTime = dateTime.AddMinutes(tupleOfHopArrivalsAndTimes.Item2[i]);
                Console.WriteLine(tupleOfHopArrivalsAndTimes.Item2[i]);
                tupleOfHopArrivalsAndTimes.Item1[i].DateTime = dateTime;
            }

            return tupleOfHopArrivalsAndTimes.Item1;
        }

        /*Warehouse FindParent(Hop child)
        {
            var level = _context.Warehouses.Find(child.Code).Level;
            int parentLevel = level + 1;
            var upperLevelWarehouses = _context.Warehouses.Where(x => x.Level == parentLevel).ToList();

            if (upperLevelWarehouses.Count == 0)
                return _context.Warehouses.Where(x => x.Level == 0).First();

            foreach (var warehouse in upperLevelWarehouses)
            {
                foreach(var baby in warehouse.NextHops)
                {
                    if (baby.Hop.Code == child.Code)
                    {
                        return warehouse;
                    }
                }
            }
            return null;
        }*/
    }
}
