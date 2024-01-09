using Org.BouncyCastle.Bcpg;

public interface IAppSettings
{
    string SecretKey { get; }
    string ConnectionString { get; }
}

public class AppSettings : IAppSettings
{
    public string SecretKey { get; }
    public string ConnectionString { get; }

    public AppSettings(IConfiguration config)
    {
        SecretKey = config["SecretKey"];
        ConnectionString = config["ConnectionStrings:DefaultConnection"];
    }
}