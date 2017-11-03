using BasketWebPanel.Custom;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
