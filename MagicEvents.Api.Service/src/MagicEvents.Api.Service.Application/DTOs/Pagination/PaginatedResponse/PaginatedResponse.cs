using System;
using System.Collections.Generic;

namespace MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public long TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext => PageIndex + 1 < TotalPages;
        public bool HasPrevious => PageIndex > 0;

        public PaginatedResponse(IEnumerable<T> items, int pageIndex, int totalPages, long count)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = totalPages;
            TotalCount = count;
        }
    }
}