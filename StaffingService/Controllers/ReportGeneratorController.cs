using StaffingService.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.IO;

namespace StaffingService.Controllers
{
    internal class ReportGeneratorController : Controller
    {
        internal ActionResult GetUserReportPdf(List<UserReport> data)
        {
            //var report = new Rotativa.MVC.ViewAsPdf("UserReport", data);
            var report = new Rotativa.MVC.PartialViewAsPdf("_UserReportPdf", data);
            string fileName = "Test.pdf";
            //var htmld = View("UserReport", data);
            report.SaveOnServerPath = Path.Combine($@"{System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath}", fileName);
            //byte[] app = report.BuildPdf(ControllerContext);
            return report;
        }

        internal string GetUserReportPdfString(List<UserReport> data)
        {
            //var htmld = new Rotativa.ViewAsPdf("UserReport", data);
            var htmld = View("UserReport", data);

            ViewData.Model = data;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(new ReportGeneratorController().ControllerContext, "UserReport");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
