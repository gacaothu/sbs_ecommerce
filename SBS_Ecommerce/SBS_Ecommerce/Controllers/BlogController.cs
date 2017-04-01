using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class BlogController : BaseController
    {
        private const string PathTheme = "~/Views/Theme/";
        private const string BlogPath = "/Blog/Index.cshtml";
        private const string BlogDetailPath = "/Blog/Detail.cshtml";
        Helper helper = new Helper();
        private SBS_Entities db = new SBS_Entities();

        // GET: Blog
        public ActionResult Index(int ?page)
        {
            int count = 4;
            var pathView = GetLayout() + BlogPath;
            var lstBlock = db.Blogs.Take(count).OrderByDescending(m=>m.UpdatedAt).ToList();
            int total = db.Blogs.Count();
            string showItem = "";
            int currentPage = 1;
            if (page != null)
            {
                var pre = int.Parse(page.ToString()) * count - count;
                var next = int.Parse(page.ToString()) * count;
                lstBlock = db.Blogs.OrderByDescending(m=>m.UpdatedAt).Skip(pre).Take(next).ToList();
                showItem = (pre + 1).ToString() + "-" + (next> total?total:next).ToString() ;
                currentPage = int.Parse(page.ToString());
            }
            else
            {
                showItem = "1-" + count.ToString();
            }

            ViewBag.NumberOfPage = (db.Blogs.Count() % count == 0 ? db.Blogs.Count() / count : db.Blogs.Count() / count + 1);
            ViewBag.Total = total;
            ViewBag.ShowItem = showItem;
            ViewBag.CurrentPage = currentPage;
            return View(pathView,lstBlock);
        }

        public ActionResult Detail(int id)
        {
            var blog = db.Blogs.Where(m => m.BlogId == id).FirstOrDefault();
            var pathView = GetLayout() + BlogDetailPath;
            return View(pathView, blog);
        }

    }
}