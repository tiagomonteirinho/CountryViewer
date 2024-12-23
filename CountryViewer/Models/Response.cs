namespace CountryViewer.Models;

public class Response
{
    public bool IsSuccessful { get; set; }

    public string Message { get; set; }

    public object Result { get; set; }
}
