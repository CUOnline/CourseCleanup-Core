using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseCleanup.Repository
{
    public abstract class RepositoryBase
    {
        protected readonly CourseCleanupContext Context;

        protected RepositoryBase(CourseCleanupContext context)
        {
            Context = context;
        }
    }
}
