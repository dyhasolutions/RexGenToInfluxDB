using DAL.Data.Repositories;
using MODELS.VehicleServerInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Datalogger> DataloggerRepo { get; }
        IRepository<DataloggerType> DataloggerTypeRepo { get; }
        IRepository<Server> ServerRepo { get; }
        IRepository<ServerCredentials> ServerCredentialsRepo { get; }
        IRepository<ServerType> ServerTypeRepo { get; }
        IRepository<Vehicle> VehicleRepo { get; }

        int Save();
    }
}
