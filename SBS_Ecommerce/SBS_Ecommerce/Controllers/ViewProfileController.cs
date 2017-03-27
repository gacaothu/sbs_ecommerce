using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ViewProfileController : Controller
    {
        // GET: ViewProfile
        public ActionResult Index()
        {
            return View();
        }

        // GET: ViewProfile/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ViewProfile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ViewProfile/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ViewProfile/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ViewProfile/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
            

    }

        // GET: ViewProfile/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ViewProfile/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
