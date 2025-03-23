namespace Models.Resps;

public class ServiceResp
{
    public bool Success { get; set; }

    public object? Content { get; set; }

    public ServiceResp(bool success, object? content = null)
    {
        Success = success;
        Content = content;
    }
}
