using System.Linq.Dynamic.Core;
using AppBase.Config.Data;
using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace AppBase.Model.Repositories;

public class SquareRepository : ISquareRepository
{
    private readonly ApiDbContext _dbContext;

    public SquareRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Utils.Paging.PagedResult<SquareResDto>> GetAll(string? name, string? description,
        Pageable pageable)
    {
        var query = _dbContext.Squares.AsQueryable();

        if (!string.IsNullOrEmpty(name)) query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(description))
            query = query.Where(r => r.Description != null && r.Description.ToLower().Contains(description.ToLower()));

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(pageable.SortBy))
        {
            var sortDirection = pageable.SortOrder?.ToLower() == "desc" ? "DESC" : "ASC";
            var sortExpression = $"{pageable.SortBy} {sortDirection}";
            query = query.OrderBy(sortExpression);
        }
        else
        {
            query = query.OrderBy("Id");
        }

        var page = pageable.Page <= 0 ? 0 : pageable.Page - 1;

        var allData = await query
            .Select(w => new SquareResDto
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                Capacity = w.Capacity,
                Geometry = (Polygon)w.Geometry
            })
            .Skip(page * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        return new Utils.Paging.PagedResult<SquareResDto>
        {
            Data = allData,
            Page = pageable.Page,
            PageSize = pageable.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageable.PageSize)
        };
    }
}