using System.Linq;
using EfCoreRepository;
using EfCoreRepository.Interfaces;
using Models;

namespace Dal.Profiles
{
    public class ConfigProfile : EntityProfile<Config>
    {
        protected override void Update(Config entity, Config dto)
        {
            entity.AuthKey = dto.AuthKey;
            entity.Value = dto.Value;
            entity.LastAccessTime = dto.LastAccessTime;
        }
    }
}