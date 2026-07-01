using Application.Services.Interfaces;
using Domain.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;

namespace tapsiriq1.Controllers.Odata
{
    [Route("odata")]
    public class OdataController : BaseOdataController
    {
        private readonly IStudentService _studentService;

        public OdataController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // EDM modeldə builder.Action("GetStudentExamReports") olaraq təyin etdiyimiz üçün URL avtomatik /odata/GetStudentExamReports olur
        [HttpPost]
        [EnableQuery]
        public IActionResult GetStudentExamReports()
        {
            IQueryable<StudentExamReport> query = _studentService.GetStudentExamReportsQueryable();
            return Ok(query);
        }
    }
}