using Microsoft.EntityFrameworkCore;
using MODELS.ErrorHAndling;
using MODELS.VehicleServerInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.VehicleServerService
{
    public class VehicleServerContext : DbContext
    {
        public DbSet<Datalogger> Dataloggers { get; set; }
        public DbSet<DataloggerType> DataloggerTypes { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerCredentials> ServerCredentials { get; set; }
        public DbSet<ServerType> ServerTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ExcpetionError> ExceptionErrors { get; set; }
        public string DbPath { get; set; }

        public string path = @"vehicle_server.db";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={path}");
    }
}
