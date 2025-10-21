namespace AppBase.API.Utils.Paging;

public class Pageable
{
    public string SortBy { get; set; } = "id";
    public string SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}