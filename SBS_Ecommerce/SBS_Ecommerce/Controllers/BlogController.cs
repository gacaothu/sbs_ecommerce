using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
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
        Helper helper = new Helper();

        // GET: Blog
        public ActionResult Index(int? page)
        {
            var theme = db.Themes.Where(m => m.CompanyId == cId && m.Active).FirstOrDefault();
            var pathView = theme.Path + BlogPath;
            var lstBlock = db.GetBlogs.Take(Count).OrderByDescending(m => m.UpdatedAt).ToList();
            int total = db.GetBlogs.Count();
            string showItem = "";
            int currentPage = 1;
            if (page != null)
            {
                var pre = int.Parse(page.ToString()) * Count - Count;
                var next = int.Parse(page.ToString()) * Count;
                lstBlock = db.GetBlogs.OrderByDescending(m => m.UpdatedAt).Take(next).Skip(pre).ToList();
                showItem = (pre + 1).ToString() + "-" + (next > total ? total : next).ToString();
                currentPage = int.Parse(page.ToString());
            }
            else
            {
                showItem = "1-" + Count.ToString();
            }

            ViewBag.NumberOfPage = (db.GetBlogs.Count() % Count == 0 ? db.GetBlogs.Count() / Count : db.GetBlogs.Count() / Count + 1);
            ViewBag.Total = total;
            ViewBag.ShowItem = showItem;
            ViewBag.CurrentPage = currentPage;
            ViewBag.ThemeName = theme.Name;
            return View(pathView, lstBlock);
        }

        public ActionResult Detail(int id)
        {
            var blog = db.GetBlogs.Where(m => m.BlogId == id).FirstOrDefault();
            var theme = db.Themes.Where(m => m.CompanyId == cId && m.Active).FirstOrDefault();
            var pathView = theme.Path + BlogDetailPath;

            ViewBag.RecentBlog = db.GetBlogs.Take(Count).OrderByDescending(m => m.UpdatedAt).ToList();
            ViewBag.Comment = db.GetBlogComments.Where(m => m.BlogId == id).ToList();
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                ViewBag.User = db.Users.Where(m => m.Id == userID).FirstOrDefault();
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
                comment.User = db.GetUsers.Where(m => m.Id == userID).FirstOrDefault();
            }
            comment.UpdatedAt = DateTime.Now;
            db.BlogComments.Add(comment);
            db.SaveChanges();

            var theme = db.Themes.Where(m => m.CompanyId == cId && m.Active).FirstOrDefault();
            return PartialView((theme.Path + "\\Blog\\_PartialComment.cshtml"), comment);
        }

    }
}