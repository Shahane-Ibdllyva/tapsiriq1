using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Domain.Models.View; // Sizin StudentExamReport-un yerləşdiyi yer
						  // Digər Vw... modellərinizin using-ləri bura əlavə olunmalıdır

namespace tapsiriq1.Extensions
{
	public static class OdataEntityInitializator
	{
		public static void ConfigureOData(this IMvcBuilder builder)
		{
			builder.AddOData(opt =>
			{
				opt.TimeZone = TimeZoneInfo.Utc;
				opt.AddRouteComponents("odata", CreateEdmModel())
					.Expand().Select()
					.Filter()
					.OrderBy()
					.Count()
					.SetMaxTop(5000);
			});
		}

		public static IEdmModel CreateEdmModel()
		{
			var builder = new ODataConventionModelBuilder();
			builder.EnableLowerCamelCase();

			// --- TƏLƏBƏ İMTAHAN HESABATI (Sizin xüsusi əlavəniz) ---
			builder.Action("GetStudentExamReports").ReturnsCollection<StudentExamReport>();

			
			return builder.GetEdmModel();
		}
	}
}