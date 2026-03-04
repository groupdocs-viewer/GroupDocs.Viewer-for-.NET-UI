using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GroupDocs.Viewer.UI.Api.Infrastructure
{
    internal class ViewerActionResult : ActionResult, IStatusCodeActionResult
    {
        public ViewerActionResult(object value)
        {
            Value = value;
        }

        public string ContentType { get; set; }

        public int? StatusCode { get; set; }

        public object Value { get; set; }
            
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = new ViewerActionResultExecutor();
            return executor.ExecuteAsync(context, this);
        }
    }
}