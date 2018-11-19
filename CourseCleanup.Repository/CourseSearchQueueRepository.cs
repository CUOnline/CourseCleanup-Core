using CourseCleanup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.Repository;

namespace CourseCleanup.Repository
{
    public class CourseSearchQueueRepository : RepositoryBase, ICourseSearchQueueRepository
    {
        public CourseSearchQueueRepository(CourseCleanupContext context) : base(context) { }

        public CourseSearchQueue Add(CourseSearchQueue model)
        {
            Context.CourseSearchQueues.Add(model);
            Context.SaveChanges();
            return model;
        }

        public CourseSearchQueue Get(int modelId)
        {
            return Context.CourseSearchQueues.Find(modelId);
        }

        public IQueryable<CourseSearchQueue> GetAll()
        {
            return Context.CourseSearchQueues;
        }

        public CourseSearchQueue Update(CourseSearchQueue model)
        {
            Context.CourseSearchQueues.Update(model);
            Context.SaveChanges();
            return model;
        }
    }
}
