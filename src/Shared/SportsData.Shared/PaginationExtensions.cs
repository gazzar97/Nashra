namespace SportsData.Shared
{
    public static class PaginationExtensions
    {
        public static Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return PagedList<T>.CreateAsync(source, pageNumber, pageSize, cancellationToken);
        }
    }
}
