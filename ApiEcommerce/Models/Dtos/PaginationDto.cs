namespace ApiEcommerce.Models.Dtos
{
    public record PaginationDTO(int PageNumber = 1, int PageSize = 10)
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; init; } = Math.Max(1, PageNumber);
        public int PageSize { get; init; } = Math.Clamp(PageSize, 1, MaxPageSize);
    }
}
