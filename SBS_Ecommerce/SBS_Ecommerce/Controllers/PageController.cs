using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class PagesController : BaseController
    {
        //private const string pathPage = "~/Content/page.xml";
        //private const string pathBlock = "~/Content/block.xml";
        //Helper helper = new Helper();

        // GET: Page
        public ActionResult Index(string id)
        {
            var page = db.GetPages.FirstOrDefault(m => m.Name == id);

            var themeName = db.Themes.Where(m => m.CompanyId == cId && m.Active).FirstOrDefault().Name;
            //var layout = "~/Views/Theme/" + cId.ToString() + "/" + themeName + "/_Layout.cshtml";
            var layout = db.GetThemes.FirstOrDefault(m => m.Active);
            if (page != null)
            {
                page.Content = ProcessContent(page.Content);
                if ((bool)page.UsingLayout)
                    ViewBag.Layout = layout.PathView + "/_Layout.cshtml";
                return View(page);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        /// <summary>
        /// Process content page
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string ProcessContent(string content)
        {
            var lstBlock = db.GetBlocks.ToList();

            foreach (var item in lstBlock)
            {
                var strReplace = "[{Block_" + item.ID.ToString() + "}]";
                content = content.Replace(@strReplace, item.Content);
            }

            return content;
        }
    }
}