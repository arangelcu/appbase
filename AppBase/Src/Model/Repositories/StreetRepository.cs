using System.Linq.Dynamic.Core;
using AppBase.Config.Data;
using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.EntityFrameworkCore;

namespace AppBase.Model.Repositories;

public class StreetRepository : IStreetRepository
{
    private readonly ApiDbContext _dbContext;

    public StreetRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Utils.Paging.PagedResult<StreetResDto>> GetAll(string? name, string? description,
        Pageable pageable)
    {
        var query = _dbContext.Streets.AsQueryable();

        if (!string.IsNullOrEmpty(name)) query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(description))
            query = query.Where(r => r.Description != null && r.Description.ToLower().Contains(description.ToLower()));

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
            .Select(w => new StreetResDto
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                Capacity = w.Capacity,
                Geometry = w.Geometry
            })
            .Skip(page * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageable.PageSize);

        return new Utils.Paging.PagedResult<StreetResDto>
        {
            Data = allData,
            Page = pageable.Page,
            PageSize = pageable.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}