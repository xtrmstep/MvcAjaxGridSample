using System.Collections.Generic;
using MvcAjaxGridSample.Types.DataModel;

namespace MvcAjaxGridSample.Types.DataStorage
{
    public interface IRepository
    {
        
    }

    public interface IRepository<T> : IRepository where T : Entity
    {
        IEnumerable<T> Get();
        T Get(int id);
        void Delete(int id);
        void Save(T item);
    }
}