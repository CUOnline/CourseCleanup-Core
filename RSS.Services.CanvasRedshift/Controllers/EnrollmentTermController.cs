using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using RSS.Services.CanvasRedshift.Models;

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

                    cmd.CommandText = "SELECT * from enrollment_term_dim";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetColumnSchema();
                            var enrollmentTerm = new EnrollmentTermDTO();
                            if (DateTime.TryParse(reader["date_start"].ToString(), out DateTime startDateValue))
                            {
                                enrollmentTerm.StartDate = startDateValue;
                            }

                            if (DateTime.TryParse(reader["date_end"].ToString(), out DateTime endDateValue))
                            {
                                enrollmentTerm.EndDate = endDateValue;
                            }

                            //enrollmentTerm.Id = long.Parse(reader["id"].ToString());
                            enrollmentTerm.Id = reader["id"].ToString();
                            enrollmentTerm.CanvasId = long.Parse(reader["canvas_id"].ToString());
                            enrollmentTerm.RootAccountId = long.Parse(reader["root_account_id"].ToString());
                            enrollmentTerm.Name = reader["name"].ToString();

                            enrollmentTerm.SisSourceId = reader["sis_source_id"].ToString();

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
