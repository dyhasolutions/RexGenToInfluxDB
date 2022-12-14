using InfluxDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.InfluxDBService
{
    public class InfluxDBService
    {
        private readonly string _token;

        public InfluxDBService()
        {
            _token = "J0xcqaPyCAQhaR65sagVtpsop3tmcFsR_BYGZbhz7gqIy883YHKc5e4aEGvtR0ZOpHjAdYM9_9xxNTUdiGAjwA==";
        }

        public async void Write(Action<WriteApi> action)
        {
            using var client = InfluxDBClientFactory.Create("http://192.168.0.126:8086", _token);
            using var write = client.GetWriteApi();
            action(write);
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
        {
            using var client = InfluxDBClientFactory.Create("http://192.168.0.126:8086", _token);
            var query = client.GetQueryApi();
            return await action(query);
        }
    }
}
