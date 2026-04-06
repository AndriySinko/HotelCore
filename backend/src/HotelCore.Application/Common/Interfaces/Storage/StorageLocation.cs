using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Common.Interfaces.Storage;

public record StorageLocation(string Bucket, string? Folder = null)
{
    public string BuildKey(string fileName) =>
        string.IsNullOrEmpty(Folder)
            ? fileName
            : $"{Folder.Trim('/')}/{fileName}";
}
