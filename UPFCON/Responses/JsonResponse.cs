namespace UPFCON.Responses;

public class JsonResponse(string message)
{
    public required string Message { get; set; } = message;
}