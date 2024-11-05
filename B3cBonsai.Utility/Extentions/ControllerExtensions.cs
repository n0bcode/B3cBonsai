using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsai.Utility.Extentions
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync(this Controller controller, string viewName, object model, bool partial = false)
        {
            var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            var tempDataProvider = controller.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            if (viewEngine == null || tempDataProvider == null)
            {
                throw new InvalidOperationException("Unable to retrieve necessary services for rendering views.");
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);
                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Unable to find view: {viewName}");
                }

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
