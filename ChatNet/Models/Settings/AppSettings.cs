namespace ChatNet.Models.Settings
{
    public class AppSettings
    {
        public DataSource DataSource { get; set; }

        public AppSettings()
        {
            DataSource = new DataSource();
        }
    }
}
