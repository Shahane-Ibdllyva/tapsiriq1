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

        [HttpPost("GetStudentExamReports")]
        [EnableQuery] 
        public IActionResult GetStudentExamReports()
        {
            
            IQueryable<StudentExamReport> query = _studentService.GetStudentExamReportsQueryable();

            return Ok(query);
        }
    }
}