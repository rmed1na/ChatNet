namespace ChatNet.Data.Models.Settings
{
    public class DataSource
    {
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }

        public string BuildSqlServerConnectionString()
            => $"Data Source={Server};" +
                $"Initial Catalog={Database};" +
                $"User ID={User};" +
                $"Password={Password};" +
                $"Connect Timeout=30;" +
                $"Encrypt=False;" +
                $"TrustServerCertificate=False;" +
                $"ApplicationIntent=ReadWrite;" +
                $"MultiSubnetFailover=False";
    }
}