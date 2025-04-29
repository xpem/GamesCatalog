namespace Models.Handlers;

public static class DeviceHandler
{
    public static LocalTestDevice CurrentDevice { get; set; }

    public enum LocalTestDevice
    {
        Windows, Emulator
    }

    public static string Url { get; set; } 
}
