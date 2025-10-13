namespace IncediosWebAPI.Model;

public class AppSettings
{
    public string? JWT { get; private set; } = "";

    private static AppSettings? _instance;
    public static AppSettings Instance
    {
        get
        {
            if(_instance is null)
                throw new NullReferenceException();
            return _instance;
        }
    }

    public static void Initialize(IConfigurationSection section)
    {
        AppSettings result = new();
        result.JWT = section.GetValue<string>("JWT");

        _instance = result;
    }
}
