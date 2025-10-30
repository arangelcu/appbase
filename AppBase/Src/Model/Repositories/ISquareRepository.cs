using AppBase.Model.Dto;
using AppBase.Utils.Paging;

namespace AppBase.Model.Repositories;

public interface ISquareRepository
{
    Task<PagedResult<SquareResDto>> GetAll(string? name, string? description, Pageable pageable);
}