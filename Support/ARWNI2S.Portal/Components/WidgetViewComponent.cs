using Microsoft.AspNetCore.Mvc;

namespace ARWNI2S.Portal.Components
{
    public partial class WidgetViewComponent : MetalinkViewComponent
    {
        private readonly IWidgetModelFactory _widgetModelFactory;

        public WidgetViewComponent(IWidgetModelFactory widgetModelFactory)
        {
            _widgetModelFactory = widgetModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);

            //no data?
            if (model.Count == 0)
                return Content("");

            return View(model);
        }
    }
}