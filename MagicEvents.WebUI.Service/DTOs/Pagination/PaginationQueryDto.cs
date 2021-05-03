namespace MagicEvents.WebUI.Service.DTOs.Pagination
{
    public class PaginationQueryDto
    {
        private const int _maximumPageSize = 20;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationQueryDto()
        {
            PageNumber = 0;
            PageSize = _maximumPageSize;
        }

        public PaginationQueryDto(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize > _maximumPageSize ? _maximumPageSize : pageSize;
        }
    }
}