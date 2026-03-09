using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using LogicTrack.Models;

namespace LogicTrack.Filters
{
    public class ApiResponseFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var sw = Stopwatch.StartNew();
            var executedContext = await next();
            sw.Stop();

            double elapsed = Math.Round(sw.Elapsed.TotalMilliseconds, 2);

            if (executedContext.Result is ObjectResult objResult)
            {
                // If value is already an ApiResponse, leave it
                var type = objResult.Value?.GetType();
                if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    // assume already wrapped
                    return;
                }

                var wrapped = new
                {
                    data = objResult.Value,
                    executionTimeMs = elapsed
                };
                executedContext.Result = new ObjectResult(wrapped) { StatusCode = objResult.StatusCode };
                return;
            }

            if (executedContext.Result is EmptyResult)
            {
                executedContext.Result = new OkObjectResult(new { executionTimeMs = elapsed });
                return;
            }

            if (executedContext.Result is StatusCodeResult statusResult)
            {
                executedContext.Result = new ObjectResult(new { status = statusResult.StatusCode, executionTimeMs = elapsed }) { StatusCode = statusResult.StatusCode };
                return;
            }

            // other result types (e.g., NotFoundResult) => convert to object result
            if (executedContext.Result is NotFoundObjectResult notFoundObj)
            {
                executedContext.Result = new NotFoundObjectResult(new { error = notFoundObj.Value ?? "Not found", executionTimeMs = elapsed });
                return;
            }

            if (executedContext.Result is NotFoundResult)
            {
                executedContext.Result = new NotFoundObjectResult(new { error = "Not found", executionTimeMs = elapsed });
                return;
            }

            // Fallback: leave result as-is
        }
    }
}
