using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ProductController : BaseController
    {
        private const string className = nameof(HomeController);
        private const string DetailsAction = "/Product/Details.cshtml";
        // GET: Product
        public ActionResult Details(int id)
        {
            var layout = GetLayout();
            var pathView = layout + DetailsAction;

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(className, methodName);
            string value = RequestUtil.SendRequest(SBSConstants.GetListProduct);
            ProductDTO result = new ProductDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }
            var product = result.Items.Where(m => m.Product_ID == id).FirstOrDefault();

            //Get product related
            var lstProductRelated = result.Items.Where(m => m.Category_ID == product.Category_ID && m.Product_ID!=id).ToList();
            ViewBag.LstProductRelated = lstProductRelated;
            return View(pathView, product);
        }
    }
}