using Alkhulasat.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alkhulasat.App.Services
{
    public class MauiAssetService : IAssetService
    {
        public async Task<string> ReadRawFileAsync(string fileName)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }

}
