using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Common.Interfaces.Storage;

public record FileUploadResult(string Key, string PublicUrl, long SizeBytes);
