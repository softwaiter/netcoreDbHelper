namespace CodeM.Common.DbHelper
{
    internal class DataSource
    {

        public DataSource(string name, string providerName, string connectionString)
        {
            this.Name = name;
            this.ProviderName = providerName;
            this.ConnectionString = connectionString;
        }

        public string Name { get; set; }

        public string ProviderName { get; set; }

        public string ConnectionString { get; set; }

    }
}
