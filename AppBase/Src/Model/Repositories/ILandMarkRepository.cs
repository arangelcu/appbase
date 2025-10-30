using AppBase.Model.Dto;
using AppBase.Utils.Paging;

namespace AppBase.Model.Repositories;

public interface ILandMarkRepository
{
    Task<PagedResult<LandMarkResDto>> GetAll(string? name, string? description, Pageable pageable);
}