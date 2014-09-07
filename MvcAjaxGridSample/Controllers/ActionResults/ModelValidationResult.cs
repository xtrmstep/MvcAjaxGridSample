using System.Linq;
using System.Net;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MvcAjaxGridSample.Controllers.ActionResults
{
    public class ModelValidationActionResult : ActionResult
    {
        public ModelValidationActionResult(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }

        public ModelStateDictionary ModelState { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var httpContext = context.RequestContext.HttpContext;
            httpContext.ClearError();

            var response = httpContext.Response;
            response.Clear();
            response.ContentEncoding = Encoding.UTF8;
            response.HeaderEncoding = Encoding.UTF8;
            response.TrySkipIisCustomErrors = true;
            response.StatusCode = (int) (ModelState.IsValid ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);

            response.ContentType = "application/json";

            var errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new
                {
                    Field = ms.Key,
                    ErrorMessage = string.Join(";", ms.Value.Errors.Select(e => e.ErrorMessage))
                }).ToArray();

            var jsonResult = Json.Encode(errors);
            var bytes = Encoding.ASCII.GetBytes(jsonResult);
            response.BinaryWrite(bytes);
        }
    }
}