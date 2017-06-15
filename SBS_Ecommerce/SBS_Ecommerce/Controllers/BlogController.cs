using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class BlogController : BaseController
    {
        private const string PathTheme = "~/Views/Theme/";
        private const string BlogPath = "/Blog/Index.cshtml";
        private const string BlogDetailPath = "/Blog/Detail.cshtml";
        private const int Count = 4;
        SBSUnitWork unitWork;

        public BlogController()
        {
            unitWork = new SBSUnitWork();
        }

        // GET: Blog
        public ActionResult Index(int? page)
        {
            var theme = GetThemeActive();
            var pathView = theme.PathView + BlogPath;
            var lstBlogsTmp = unitWork.Repository<Blog>().GetAll(m => m.CompanyId == cId);
            var lstBlog = lstBlogsTmp.OrderByDescending(m => m.UpdatedAt).Take(Count).ToList();
            int total = lstBlogsTmp.Count();
            string showItem = "";
            int currentPage = 1;
            if (page != null)
            {
                var pre = int.Parse(page.ToString()) * Count - Count;
                var next = int.Parse(page.ToString()) * Count;
                lstBlog = lstBlogsTmp.OrderByDescending(m => m.UpdatedAt).Take(next).Skip(pre).ToList();
                showItem = (pre + 1).ToString() + "-" + (next > total ? total : next).ToString();
                currentPage = int.Parse(page.ToString());
            }
            else
            {
                showItem = "1-" + Count.ToString();
            }

            ViewBag.NumberOfPage = (lstBlogsTmp.Count() % Count == 0 ? lstBlogsTmp.Count() / Count : lstBlogsTmp.Count() / Count + 1);
            ViewBag.Total = total;
            ViewBag.ShowItem = showItem;
            ViewBag.CurrentPage = currentPage;
            ViewBag.ThemeName = theme.Name;

            // SEO
            InitSEO(Request.Url.Scheme, Request.Url.Host, Request.FilePath);
            return View(pathView, lstBlog);
        }

        public ActionResult Detail(int id)
        {
            var blog = unitWork.Repository<Blog>().Find(id);
            var theme = GetThemeActive();
            var pathView = theme.PathView + BlogDetailPath;

            ViewBag.RecentBlog = unitWork.Repository<Blog>().GetAll(m => m.CompanyId == cId).Take(Count).OrderByDescending(m => m.UpdatedAt).ToList();
            ViewBag.Comment = unitWork.Repository<BlogComment>().GetAll(m => m.BlogId == id).ToList();
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                ViewBag.User = unitWork.Repository<User>().Find(userID);
            }
            return View(pathView, blog);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddComment(string name, string email, string message, int blogID)
        {
            BlogComment comment = new BlogComment();
            comment.BlogId = blogID;
            comment.Content = message;
            comment.Name = name;
            comment.Email = email;
            comment.CreatedAt = DateTime.Now;
            comment.Status = "1";
            var userID = GetIdUserCurrent();

            if (userID != -1)
            {
                comment.UId = userID;
                comment.User = unitWork.Repository<User>().Find(userID);
            }
            comment.UpdatedAt = DateTime.Now;
            unitWork.Repository<BlogComment>().Add(comment);
            unitWork.SaveChanges();
            var theme = GetThemeActive();
            return PartialView((theme.PathView + "\\Blog\\_PartialComment.cshtml"), comment);
        }
    }
}