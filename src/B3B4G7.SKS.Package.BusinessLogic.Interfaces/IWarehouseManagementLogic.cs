using B3B4G7.SKS.Package.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces
{
    public interface IWarehouseManagementLogic
    {
        public Warehouse ExportWarehouses();
        public Hop GetHop(string code);
        public void ImportWarehouses(Warehouse warehouse);
    }
}
