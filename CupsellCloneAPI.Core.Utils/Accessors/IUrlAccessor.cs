namespace CupsellCloneAPI.Core.Utils.Accessors
{
    public interface IUrlAccessor
    {
        string BaseUrl { get; }
        string GetControllerPath(string controllerName);
    }
}