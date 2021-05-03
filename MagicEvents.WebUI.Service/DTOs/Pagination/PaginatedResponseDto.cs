using System;
using System.Collections.Generic;

namespace MagicEvents.WebUI.Service.DTOs.Pagination
{
    public class PaginatedResponseDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public long TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}