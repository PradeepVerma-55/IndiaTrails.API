using IndiaTrails.API.Data;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace IndiaTrails.API.Repositories
{
    public class SqlServerWalkRepository : IWalkRepository
    {
        private readonly IndiaTrailsDBContext _dbContext;

        public SqlServerWalkRepository(IndiaTrailsDBContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await _dbContext.Walks.AddAsync(walk);
            await _dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var walk = _dbContext.Walks.Find(id);
            if (walk == null)
                return null;
            _dbContext.Walks.Remove(walk);
            await _dbContext.SaveChangesAsync();
            return walk;

        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy= null, bool isAscending = true)
        {
            var walks = _dbContext.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .AsQueryable();

            // filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    // ✅ EF-safe, case-insensitive filter
                    walks = walks.Where(x => x.Name.ToLower().Contains(filterQuery.ToLower()));
                }
                else if (filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
                {
                    // ✅ EF-safe, case-insensitive filter
                    walks = walks.Where(x => x.Description.ToLower().Contains(filterQuery.ToLower()));
                }
            }

            // Sorting 
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            return await walks.ToListAsync();
        }


        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existingWalk = await _dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingWalk == null)
            {
                return null;
            }

            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;

            await _dbContext.SaveChangesAsync();
            return existingWalk;
        }
    }
}
