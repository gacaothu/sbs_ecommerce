using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class PagesController : BaseController
    {
        private SBSUnitWork unitWork;

        public PagesController()
        {
            unitWork = new SBSUnitWork();
        }

        // GET: Page
        public async Task<ActionResult> Index(string id)
        {
            var page = await unitWork.Repository<Page>().GetAsync(m=>m.Name == id);
            var layout = GetThemeActive();
            if (page != null)
            {
                ViewData["Keywords"] = page.Keyword;
                ViewData["Description"] = page.Description;

                page.Content = ProcessContent(page.Content);
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
            //var lstBlock = db.GetBlocks.ToList();
            var lstBlock = unitWork.Repository<Block>().GetAll(m => m.CompanyId == cId).ToList();

            foreach (var item in lstBlock)
            {
                var strReplace = "[{Block_" + item.ID.ToString() + "}]";
                content = content.Replace(@strReplace, item.Content);
            }

            return content;
        }
    }
}