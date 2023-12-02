using B3B4G7.SKS.Package.DataAccess.Entities;

namespace B3B4G7.SKS.Package.DataAccess.Interfaces
{
    public interface IWarehouseRepository
    {
        Warehouse Create(Warehouse warehouse);
        Warehouse GetWarehouses();
        Hop GetByCode(string code);
        void ClearDatabase();
    }
}
