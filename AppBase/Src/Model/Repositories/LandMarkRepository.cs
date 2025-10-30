using System.Linq.Dynamic.Core;
using AppBase.Config.Data;
using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace AppBase.Model.Repositories;

public class LandMarkRepository : ILandMarkRepository
{
    private readonly ApiDbContext _dbContext;

    public LandMarkRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Utils.Paging.PagedResult<LandMarkResDto>> GetAll(string? name, string? description,
        Pageable pageable)
    {
        var query = _dbContext.LandMarks.AsQueryable();

        if (!string.IsNullOrEmpty(name)) query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(description))
            query = query.Where(r => r.Description.ToLower().Contains(description.ToLower()));

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(pageable.SortBy))
        {
            var sortExpression = pageable.SortOrder?.ToLower() == "desc"
                ? $"{pageable.SortBy} DESC"
                : pageable.SortBy;

            query = query.OrderBy(sortExpression);
        }
        else
        {
            query = query.OrderBy(wt => wt.Id);
        }

        var page = pageable.Page - 1;
        if (page < 0) page = 0;

        var allData = await query
            .Select(w => new LandMarkResDto
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                Geometry = (Point)w.Geometry
            })
            .Skip(page * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageable.PageSize);

        return new Utils.Paging.PagedResult<LandMarkResDto>
        {
            Data = allData,
            Page = pageable.Page,
            PageSize = pageable.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}