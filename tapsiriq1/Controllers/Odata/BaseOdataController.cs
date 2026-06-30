//using Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace tapsiriq1.Controllers.Odata
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(ODataActionFilter))] // Bizim yaratdığımız xüsusi konfiqurasiya filteri
    public abstract class BaseOdataController : ODataController
    {
        // Bütün ortaq metodlar, loqlar və s. bura əlavə oluna bilər
    }
}