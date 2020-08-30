using System.Threading.Tasks;
using Models;

namespace Logic.Interfaces
{
    public interface IConfigLogic
    {
        public Task<string> Create();

        public Task<Config> Update(string key, string value);

        public Task<string> Load(string key);
        
        public Task Delete(string key);

        public Task Cleanup();
    }
}