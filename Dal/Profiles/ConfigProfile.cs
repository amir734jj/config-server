using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal.Profiles
{
    public class ConfigProfile : IEntityProfile<Config, int>
    {
        public Config Update(Config entity, Config dto)
        {
            entity.Key = dto.Key;
            entity.Value = dto.Value;
            entity.LastAccessTime = dto.LastAccessTime;
            
            return entity;
        }

        public IQueryable<Config> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Config>
        {
            return queryable.Include(x => x.Value);
        }
    }
}