using Microsoft.EntityFrameworkCore;
namespace NexoraBackend.Common.Models;

//generic panagination helper
public class PagedList<T>
{

    //here <T> means for any type(User, Product, Order etc.) 
    // we can create a paged list of that type
    //eg PagedList<User> , PagedList<Product> etc.

    public IReadOnlyList<T> Items { get; }  //actual data of the current page
    public int PageNumber { get; } //current page number
    public int PageSize { get; } // how many items per page
    public int TotalCount { get; } //total number of items across all pages

    //it calculates the total number of pages based on the total count and page size.
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    //If you're not on page 1 → previous page exists
    // page 3 -> true
    // page 1 -> false
    public bool HasPreviousPage => PageNumber > 1;

    //If current page is not last page → next page exists
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await source.CountAsync(cancellationToken); // counts the total number of items in the source query

        //pageNumber = 3
        //pageSize = 10
        // skip (3-1)*10 = 20 items and take next 10 items
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);
        //A CancellationToken is a way to stop an ongoing async operation before it finishes.
        // Think of it like a “stop button” for running tasks. “Stop this operation if I no longer need the result.”


        return new PagedList<T>(items, total, pageNumber, pageSize);
    }

}