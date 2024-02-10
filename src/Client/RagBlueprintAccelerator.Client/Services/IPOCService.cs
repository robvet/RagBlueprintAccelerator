namespace RagBlueprintAccelerator.Client.Services;

public interface IPOCService
{
    Task<string> CallPOCServiceGet();
    Task<string> CallPOCServicePost();
}
