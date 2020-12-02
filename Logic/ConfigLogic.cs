using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using AuthenticatedEncryption;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
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
            var cryptKey = Encryption.NewKey();
            var authKey = Encryption.NewKey();
            
            var cryptKeyBase64String = Convert.ToBase64String(cryptKey);
            var authKeyBase64String = Convert.ToBase64String(authKey);

            var config = await _configDal.Save(new Config
            {
                AuthKey = authKeyBase64String,
                Value = Encryption.Encrypt(JsonSerializer.Serialize(new { }), cryptKey, authKey),
                LastAccessTime = DateTimeOffset.Now
            });

            return PackKey(config.Id, Sha256ToString(authKeyBase64String), cryptKeyBase64String);
        }

        public async Task<Config> Update(string key, string value)
        {
            var (id, authKeyHash, cryptKey) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && Sha256ToString(config.AuthKey) == authKeyHash)
            {
                config.LastAccessTime = DateTimeOffset.Now;
                
                config.Value = Encryption.Encrypt(value, Convert.FromBase64String(cryptKey), Convert.FromBase64String(config.AuthKey));
                
                return await _configDal.Update(config.Id, config);
            }

            return null;
        }

        public async Task<string> Load(string key)
        {
            var (id, authKeyHash, cryptKey) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && Sha256ToString(config.AuthKey) == authKeyHash)
            {
                config.LastAccessTime = DateTimeOffset.Now;
                
                await _configDal.Update(config.Id, config);
                
                var plainText = Encryption.Decrypt(config.Value, Convert.FromBase64String(cryptKey), Convert.FromBase64String(config.AuthKey));

                return plainText;
            }

            return null;
        }

        public async Task Delete(string key)
        {
            var (id, authKeyHash, _) = UnpackKey(key);

            var config = await _configDal.Get(id);

            if (config != null && Sha256ToString(config.AuthKey) == authKeyHash)
            {
                await _configDal.Delete(config.Id);
            }
        }

        public async Task Cleanup()
        {
            var now = DateTimeOffset.Now;

            await Task.WhenAll((await _configDal.GetAll())
                .Where(x => now - x.LastAccessTime > TimeSpan.FromDays(90))
                .Select(config => _configDal.Delete(config.Id)));
        }

        public async Task<object> Status()
        {
            return new
            {
                Count = (await _configDal.GetAll()).Count()
            };
        }

        private static string PackKey(int id, string authKeyHash, string cryptKey)
        {
            return HttpUtility.UrlEncode(string.Join(Delimiter, id, authKeyHash, cryptKey));
        }

        private static (int id, string authKeyHash, string cryptKey) UnpackKey(string key)
        {
            var result = HttpUtility.UrlDecode(key).Split(Delimiter);

            if (result.Length != 3)
            {
                throw new Exception("Failed to unpack the combination key");
            }

            return (int.Parse(result[0]), result[1], result[2]);
        }

        private static string Sha256ToString(string text)
        {
            using var sha = new SHA256Managed();
            var textData = Encoding.UTF8.GetBytes(text);
            var hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}