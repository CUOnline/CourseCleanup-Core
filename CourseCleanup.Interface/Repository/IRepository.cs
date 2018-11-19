using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseCleanup.Interface.Repository
{
    public interface IRepository<T>
    {
        T Add(T model);
        IQueryable<T> GetAll();
        T Get(int modelId);
        T Update(T model);
    }
}
