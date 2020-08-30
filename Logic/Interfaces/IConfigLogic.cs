using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IConfigLogic
    {
        public Task<string> Create();

        public Task Update(string key, string value);

        public Task<string> Load(string key);
    }
}