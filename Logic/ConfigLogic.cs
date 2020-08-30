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

        public async Task Update(string key, string value)
        {
            var (id, rawKey) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && config.Key == rawKey)
            {
                config.LastAccessTime = DateTimeOffset.Now;
                
                config.Value = value;

                await _configDal.Update(config.Id, config);
            }
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

        private static string PackKey(int id, string rawKey)
        {
            return $"{id}/{rawKey}";
        }

        private static (int id, string rawKey) UnpackKey(string key)
        {
            var result = key.Split("/");

            return (int.Parse(result.First()), result.Last());
        }
    }
}