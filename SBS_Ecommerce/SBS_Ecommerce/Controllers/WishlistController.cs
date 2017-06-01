using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    [Authorize]
    public class WishlistController : BaseController
    {
        private const string ClassName = nameof(WishlistController);
        private const string PathWishlist = "/Wishlist/Wishlist.cshtml";
        private const string PathLogin = "/Account/Login.cshtml";

        /// <summary>
        /// Gets the Wishlist. 
        /// </summary>
        /// <returns></returns>
        public ActionResult Wishlist()
        {
            var uId = GetIdUserCurrent();
            if (uId == SBSConstants.Failed)
            {
                return View(GetLayout() + PathLogin);
            }

            var wishList = db.GetWishlists.Where(m => m.UId == uId).ToList();
            List<WishlistDTO> result = new List<WishlistDTO>();
            string value = "";
            foreach (var item in wishList)
            {
                value = RequestUtil.SendRequest(string.Format(SBSConstants.GetProduct, item.ProId));
                var product = JsonConvert.DeserializeObject<ProductDetailDTO>(value);
                result.Add(new WishlistDTO()
                {
                    Id = item.Id,
                    ProId = item.ProId,
                    CompanyId = item.CompanyId,
                    UId = item.UId,
                    Status = item.Status,
                    Product = product.Items
                });
                ViewBag.Data = result;
            }
            var viewPath = GetLayout() + PathWishlist;

            return View(viewPath, ViewBag.Data);
        }

        /// <summary>
        /// Inserts to Wishlist.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult InsertToWishlist(int id)
        {
            int userId = GetIdUserCurrent();
            if (userId == SBSConstants.Failed)
            {
                return Json(new { reponse = SBSConstants.LoginRequired });
            }
            try
            {
                var item = db.GetWishlists.Where(m => m.ProId == id && m.UId == userId).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { reponse = SBSConstants.Exists });
                }
                db.Wishlists.Add(new Wishlist { CompanyId = cId, UId = userId, ProId = id, Status = SBSConstants.Active });
                db.SaveChanges();
                return Json(new { reponse = SBSConstants.Success });
            }
            catch (Exception e)
            {
                return Json(new { response = SBSConstants.Failed, Message = e.Message });
            }
        }

        /// <summary>
        /// Removes from Wishlist.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromWishlist(int id)
        {
            try
            {
                var item = new Wishlist() { Id = id };
                db.Wishlists.Attach(item);
                db.Wishlists.Remove(item);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { response = SBSConstants.Failed, Message = e.Message });
            }

            return Json(new { reponse = SBSConstants.Success });
        }
    }
}