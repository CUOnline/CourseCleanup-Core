using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using RSS.Providers.CanvasRedshift.Models;

namespace RSS.Providers.CanvasRedshift.Controllers
{
    [Route("api/[controller]")]
    public class EnrollmentTermController : ControllerBase
    {
        private readonly AppSettings appSettings;
        public EnrollmentTermController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }


        [HttpGet]
        public ActionResult Get()
        {
            var result = new List<EnrollmentTermDTO>();

            var conn = new NpgsqlConnection(appSettings.RedshiftConnectionString);
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT id, name, date_start, date_end from enrollment_term_dim";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var enrollmentTerm = new EnrollmentTermDTO();
                            if (DateTime.TryParse(reader["date_start"].ToString(), out DateTime startDateValue))
                            {
                                enrollmentTerm.StartDate = startDateValue;
                            }

                            if (DateTime.TryParse(reader["date_end"].ToString(), out DateTime endDateValue))
                            {
                                enrollmentTerm.EndDate = endDateValue;
                            }

                            enrollmentTerm.Id = reader["id"].ToString();
                            enrollmentTerm.Name = reader["name"].ToString();

                            result.Add(enrollmentTerm);
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
