using AppBase.Model.Dto;
using AppBase.Utils.Paging;

namespace AppBase.Model.Repositories;

public interface IStreetRepository
{
    Task<PagedResult<StreetResDto>> GetAll(string? name, string? description, Pageable pageable);
}