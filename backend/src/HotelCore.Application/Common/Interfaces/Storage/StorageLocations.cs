namespace HotelCore.Application.Common.Interfaces.Storage;

public static class StorageLocations
{
    public static class Buckets
    {
        public const string Public = "public";
        public const string Private = "private";
    }

    public static class Folders
    {
        public const string OrderImages = "orders";
        public const string ProfileImages = "profiles";
        public const string ProjectImages = "projects";
        public const string ChatImages = "chat/images";
        public const string ChatFiles = "chat/files";
        public const string Certificates = "certificates";
        public const string Invoices = "invoices";
        public const string Other = "other";
        public const string Categories = "categories";
    }

    public static readonly StorageLocation OrderImages = new(Buckets.Public, Folders.OrderImages);
    public static readonly StorageLocation ProfileImages = new(Buckets.Public, Folders.ProfileImages);
    public static readonly StorageLocation ProjectImages = new(Buckets.Public, Folders.ProjectImages);
    public static readonly StorageLocation ChatImages = new(Buckets.Public, Folders.ChatImages);
    public static readonly StorageLocation ChatFiles = new(Buckets.Public, Folders.ChatFiles);
    public static readonly StorageLocation Certificates = new(Buckets.Public, Folders.Certificates);
    public static readonly StorageLocation Invoices = new(Buckets.Public, Folders.Invoices);
    public static readonly StorageLocation Other = new(Buckets.Public, Folders.Other);
    public static readonly StorageLocation Categories = new(Buckets.Public, Folders.Categories);
}
