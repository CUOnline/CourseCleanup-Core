using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using RSS.Services.CanvasRedshift.Models;

namespace RSS.Services.CanvasRedshift.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        private readonly AppSettings appSettings;
        public CoursesController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpGet("GetUnusedCourses")]
        public IActionResult GetUnusedCourses(long termId)
        {
            // Get all courses that have been unpublished for the termId.
            // If any course has 1. No syllabusbody, 2. No enrollments (LastActivityAt == null),
            // 3. No assignments, 4. No modules, 5. No Files, 6. No Pages, 7. No Discussions, and 8. No Quizzes.. it should be returned.
            var result = new List<UnusedCourseDTO>();

            var conn = new NpgsqlConnection(appSettings.CanvasRedshiftConnectionString);
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "select * from course_dim where course_dim.id in"
                                      + " (SELECT course.id from course_dim course"
                                      + " left outer join enrollment_dim enrollment on enrollment.course_id = course.id"
                                      + " left outer join assignment_dim assignment on assignment.course_id = course.id"
                                      + " left outer join module_dim module on module.course_id = course.id"
                                      + " left outer join file_dim file on file.course_id = course.id"
                                      + " left outer join wiki_page_fact wiki on wiki.parent_course_id = course.id"
                                      + " left outer join discussion_topic_dim discussion on discussion.course_id = course.id"
                                      + " left outer join quiz_dim quiz on quiz.course_id = course.id"
                                      + $" where course.enrollment_term_id='{termId}' and (course.workflow_state = 'created' or course.workflow_state = 'claimed') and char_length(coalesce(syllabus_body, '')) = 0"
                                      + " and (enrollment.course_id is null or enrollment.last_activity_at is null)"
                                      + " and assignment.course_id is null"
                                      + " and module.course_id is null"
                                      + " and file.course_id is null"
                                      + " and wiki.parent_course_id is null"
                                      + " and discussion.course_id is null"
                                      + " and quiz.course_id is null group by course.id)";
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var unusedCourse = new UnusedCourseDTO()
                            {
                                Id = long.Parse(reader["id"].ToString()),
                                CanvasId = long.Parse(reader["canvas_id"].ToString()),
                                EnrollmentTermId = reader["enrollment_term_id"].ToString(),
                                Name = reader["name"].ToString(),
                                Code = reader["code"].ToString(),
                                //SisCourseId = reader["sis_course_id"].ToString()
                                SisCourseId = reader["sis_source_id"].ToString(),
                                AccountId = reader["account_id"].ToString()
                            };

                            if (DateTime.TryParse(reader["created_at"].ToString(), out DateTime createdDate))
                            {
                                unusedCourse.CreatedAt = createdDate;
                            }

                            result.Add(unusedCourse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            finally
            {
                if (conn.FullState == ConnectionState.Open)
                    conn.Close();

                conn.Dispose();
            }

            return Ok(result);
        }
    }
}
