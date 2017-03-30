using SBS_Ecommerce.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class PagesController : Controller
    {
        private const string pathPage = "~/Content/page.xml";
        private const string pathBlock = "~/Content/block.xml";
        Helper helper = new Helper();

        // GET: Page
        public ActionResult Index(int id)
        {
            var lstPage = helper.DeSerializePage(Server.MapPath(pathPage));
            var page = lstPage.Where(m => m.ID == id).FirstOrDefault();
            page.Content = ProcessContent(page.Content);
            return View(page);
        }

        /// <summary>
        /// Process content page
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string ProcessContent(string content)
        {
            var lstBlock = helper.DeSerializeBlock(Server.MapPath(pathBlock));

            foreach (var item in lstBlock)
            {
                var strReplace = "[{Block_" + item.ID.ToString() + "}]";
                content = content.Replace(@strReplace, item.Content);
            }

            return content;
        }
    }
}