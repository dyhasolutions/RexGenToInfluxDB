using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.InfluxDBService
{
    public class InfluxDBService
    {
        public InfluxDBService() { }

        public async void Write(Action<WriteApi> action, string IP_URL, string token)
        {
            using var client = InfluxDBClientFactory.Create(IP_URL, token);
            using var write = client.GetWriteApi();
            action(write);
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action, string IP_URL, string token)
        {
            using var client = InfluxDBClientFactory.Create(IP_URL, token);
            var query = client.GetQueryApi();
            return await action(query);
        }
    }
}
