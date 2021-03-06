﻿using CourseCleanup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseCleanup.Interface.BLL
{
    public interface ICourseSearchQueueBLL : IBLL<CourseSearchQueue>
    {
        CourseSearchQueue GetNextSearchToProcess();
    }
}
