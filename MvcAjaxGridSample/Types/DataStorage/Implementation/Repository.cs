using System.Collections.Generic;
using System.Linq;
using MvcAjaxGridSample.Types.DataModel;

namespace MvcAjaxGridSample.Types.DataStorage.Implementation
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly Dictionary<int, T> _internalStorage = new Dictionary<int, T>(); 

        public IEnumerable<T> Get()
        {
            return _internalStorage.Values;
        }

        public T Get(int id)
        {
            return _internalStorage.ContainsKey(id) ? _internalStorage[id] : default(T);
        }

        public void Delete(int id)
        {
            _internalStorage.Remove(id);
        }

        public void Save(T item)
        {
            if (item.Id < 0) // that means 'not stored and new'
            {
                item.Id = GetNewId();
                _internalStorage.Add(item.Id, item);
            }
            else
                _internalStorage[item.Id] = item;
        }

        private int GetNewId()
        {
            if (_internalStorage.Any())
                return _internalStorage.Values.Max(t => t.Id) + 1;
            return 1;
        }
    }
}