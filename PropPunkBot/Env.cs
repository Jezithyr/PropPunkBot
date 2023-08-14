namespace PropPunkBot;

public static class Env
{
    private static bool _loaded;

    public static void EnsureLoadEnvFile()
    {
        if (_loaded)
            return;

        _loaded = true;

        var lines = File.ReadAllLines(".env");
        foreach (var line in lines)
        {
            var str = line.Split('=');
            var key = str[0];
            var val = str[1];

            Environment.SetEnvironmentVariable(key, val);
        }
    }
}
