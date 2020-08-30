using System;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using MlkPwgen;
using Models;

namespace Logic
{
    public class ConfigLogic : IConfigLogic
    {
        private const char Delimiter = '-';
        
        private readonly IBasicCrudType<Config, int> _configDal;

        public ConfigLogic(IEfRepository repository)
        {
            _configDal = repository.For<Config, int>();
        }

        public async Task<string> Create()
        {
            var password = PasswordGenerator.Generate(length: 256, allowed: Sets.Alphanumerics);

            var config = await _configDal.Save(new Config {Key = password});

            return PackKey(config.Id, password);
        }

        public async Task<Config> Update(string key, string value)
        {
            var (id, rawKey) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && config.Key == rawKey)
            {
                config.LastAccessTime = DateTimeOffset.Now;
                
                config.Value = value;

                return await _configDal.Update(config.Id, config);
            }

            return null;
        }

        public async Task<string> Load(string key)
        {
            var (id, rawKey) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && config.Key == rawKey)
            {
                config.LastAccessTime = DateTimeOffset.Now;
                
                await _configDal.Update(config.Id, config);

                return config.Value;
            }

            return null;
        }

        public async Task Delete(string key)
        {
            var (id, rawKey) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && config.Key == rawKey)
            {
                await _configDal.Delete(config.Id);
            }
        }

        private static string PackKey(int id, string rawKey)
        {
            return $"{id}{Delimiter}{rawKey}";
        }

        private static (int id, string rawKey) UnpackKey(string key)
        {
            var result = key.Split(Delimiter);

            return (int.Parse(result.First()), result.Last());
        }
    }
}