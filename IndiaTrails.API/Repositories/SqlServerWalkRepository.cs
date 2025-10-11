using IndiaTrails.API.Data;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Walk>> GetAllAsync()
        {
            return await _dbContext.Walks.ToListAsync();
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
