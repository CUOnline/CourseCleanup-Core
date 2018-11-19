using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;

namespace CourseCleanup.BLL
{
    public class CourseSearchQueueBLL : ICourseSearchQueueBLL
    {
        private readonly ICourseSearchQueueRepository courseSearchQueueRepository;

        public CourseSearchQueueBLL(ICourseSearchQueueRepository courseSearchQueueRepository)
        {
            this.courseSearchQueueRepository = courseSearchQueueRepository;
        }

        public CourseSearchQueue Add(CourseSearchQueue model)
        {
            model.DateCreated = DateTime.Now;
            return courseSearchQueueRepository.Add(model);
        }

        public CourseSearchQueue Get(int modelId)
        {
            return courseSearchQueueRepository.Get(modelId);
        }

        public IQueryable<CourseSearchQueue> GetAll()
        {
            return courseSearchQueueRepository.GetAll();
        }

        public CourseSearchQueue GetNextSearchToProcess()
        {
            return courseSearchQueueRepository.GetAll().Where(x => x.Status == SearchStatus.New).OrderBy(x => x.DateCreated).FirstOrDefault();
        }

        public CourseSearchQueue Remove(CourseSearchQueue model)
        {
            throw new NotImplementedException();
        }

        public CourseSearchQueue Update(CourseSearchQueue model)
        {
            model.LastUpdated = DateTime.Now;
            return courseSearchQueueRepository.Update(model);
        }
    }
}
