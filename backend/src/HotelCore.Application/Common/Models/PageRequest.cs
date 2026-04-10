// This file contains code for PageRequest.
namespace HotelCore.Application.Common.Models;

public record PageRequest
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private int _page = DefaultPage;
    private int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        init => _page = value < 1 ? DefaultPage : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value < 1 ? DefaultPageSize : Math.Min(value, MaxPageSize);
    }

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
    public static PageRequest Default => new();
    public static PageRequest Create(int page, int pageSize) => new() { Page = page, PageSize = pageSize };
}
