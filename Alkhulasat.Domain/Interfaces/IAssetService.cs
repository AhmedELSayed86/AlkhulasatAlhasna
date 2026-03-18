namespace Alkhulasat.Domain.Interfaces
{
    public interface IAssetService
    {
        Task<string> ReadRawFileAsync(string fileName);
    }

}
