using DotNetEnv;

namespace Obilet.JourneySearch.Web.Configuration;

public static class EnvConfiguration
{
    static EnvConfiguration()
    {
        try
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

            if (File.Exists(envPath))
            {
                Env.TraversePath().Load(envPath);
                Console.WriteLine(".env file loaded successfully.");
            }
            else
            {
                Console.WriteLine($"Warning: .env file not found. {envPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Configuration error: {ex}");
        }
    }

    public static string OBILET_API_BASE_URL => Env.GetString("OBILET_API_BASE_URL");
    public static string OBILET_API_CLIENT_TOKEN => Env.GetString("OBILET_API_CLIENT_TOKEN");
    public static string OBILET_API_APPLICATION_VERSION => Env.GetString("OBILET_API_APPLICATION_VERSION");
    public static string OBILET_API_EQUIPMENT_ID => Env.GetString("OBILET_API_EQUIPMENT_ID");
    public static string OBILET_API_LANGUAGE => Env.GetString("OBILET_API_LANGUAGE");
}