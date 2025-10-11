using IndiaTrails.API.Data;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace IndiaTrails.API.Repositories
{
    public class SqlServerRegionRepository : IRegionRepository
    {
        private readonly IndiaTrailsDBContext _dbContext;

        public SqlServerRegionRepository(IndiaTrailsDBContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<Region> CreateAsync(Region region)
        {
            await _dbContext.Regions.AddAsync(region);
            await _dbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
           var region= _dbContext.Regions.Find(id);
            if (region == null)
                return null;
            _dbContext.Regions.Remove(region);
            await _dbContext.SaveChangesAsync();
            return region;

        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await _dbContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var existingRegion = await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRegion == null)
            {
                return null;
            }

            existingRegion.Code = region.Code;
            existingRegion.Name = region.Name;
            existingRegion.RegionImageUrl = region.RegionImageUrl;

            await _dbContext.SaveChangesAsync();
            return existingRegion;
        }
    }
}
