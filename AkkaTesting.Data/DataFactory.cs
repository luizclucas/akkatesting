using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Threading.Tasks;

namespace AkkaTesting.Data
{
    public class DataFactory
    {
        private readonly string _mysqlCn;
        private DbConnection _connection;
        
        public DataFactory()
        {
            _mysqlCn = "Server=remotemysql.com; port=3306; Uid=NgJ1KreqYM; Pwd=tS3PanMqA2; Database=NgJ1KreqYM; Connect Timeout=120;";
            _connection = CreateConnection();
            _connection.Open();
        }

        public DbConnection CreateConnection() => new MySqlConnection(_mysqlCn);

        public DbConnection OpenConnection()
        {
            var c = CreateConnection();
            c.Open();
            return c;
        }

        public async Task<DbConnection> OpenConnectionAsync()
        {
            var c = CreateConnection();
            await c.OpenAsync();
            return c;
        }

        public async Task<DbConnection> OpenConnectionIfNotAliveAsync()
        {
            if(_connection == null || _connection?.State == System.Data.ConnectionState.Closed || _connection.State == System.Data.ConnectionState.Broken)
            {             
                await _connection.OpenAsync();
            }
            return _connection;           
        }

    }
}
