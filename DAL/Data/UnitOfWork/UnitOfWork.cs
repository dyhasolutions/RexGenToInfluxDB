using DAL.Data.Repositories;
using DAL.VehicleServerService;
using MODELS.ErrorHAndling;
using MODELS.VehicleServerInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Attributes
        private IRepository<Datalogger> _dataloggerRepo;
        private IRepository<DataloggerType> _dataloggerTypeRepo;
        private IRepository<Server> _serverRepo;
        private IRepository<ServerCredentials> _serverCredentialsRepo;
        private IRepository<ServerType> _serverTypeRepo;
        private IRepository<Vehicle> _vehicleRepo;
        private IRepository<ExcpetionError> _exceptionError;
        #endregion

        #region Repositories
        public IRepository<ExcpetionError> ExceptionErrorRepo
        {
            get
            {
                if (_exceptionError == null)
                {
                    _exceptionError = new Repository<ExcpetionError>(this.VehicleServerContext);
                }
                return _exceptionError;
            }
        }
        public IRepository<Datalogger> DataloggerRepo
        {
            get
            {
                if (_dataloggerRepo == null)
                {
                    _dataloggerRepo = new Repository<Datalogger>(this.VehicleServerContext);
                }
                return _dataloggerRepo;
            }
        }
        public IRepository<DataloggerType> DataloggerTypeRepo
        {
            get
            {
                if (_dataloggerTypeRepo == null)
                {
                    _dataloggerTypeRepo = new Repository<DataloggerType>(this.VehicleServerContext);
                }
                return _dataloggerTypeRepo;
            }
        }
        public IRepository<Server> ServerRepo
        {
            get
            {
                if (_serverRepo == null)
                {
                    _serverRepo = new Repository<Server>(this.VehicleServerContext);
                }
                return _serverRepo;
            }
        }
        public IRepository<ServerCredentials> ServerCredentialsRepo
        {
            get
            {
                if (_serverCredentialsRepo == null)
                {
                    _serverCredentialsRepo = new Repository<ServerCredentials>(this.VehicleServerContext);
                }
                return _serverCredentialsRepo;
            }
        }
        public IRepository<ServerType> ServerTypeRepo
        {
            get
            {
                if (_serverTypeRepo == null)
                {
                    _serverTypeRepo = new Repository<ServerType>(this.VehicleServerContext);
                }
                return _serverTypeRepo;
            }
        }
        public IRepository<Vehicle> VehicleRepo
        {
            get
            {
                if (_vehicleRepo == null)
                {
                    _vehicleRepo = new Repository<Vehicle>(this.VehicleServerContext);
                }
                return _vehicleRepo;
            }
        }
        #endregion

        private VehicleServerContext VehicleServerContext { get; }
        public UnitOfWork(VehicleServerContext vehicleServerContext)
        {
            this.VehicleServerContext = vehicleServerContext;
        }

        public void Dispose()
        {
            VehicleServerContext.Dispose();
        }

        public int Save()
        {
            return VehicleServerContext.SaveChanges();
        }
    }
}
