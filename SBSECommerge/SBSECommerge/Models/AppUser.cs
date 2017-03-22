using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SBSECommerge.Models
{
    public class AppUser : ClaimsPrincipal
    {
        public AppUser(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public string Name
        {
            get
            {
                var claimActor = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Actor);
                var actor = (claimActor == null ? string.Empty : claimActor.Value);
                return actor;
            }
        }

    }
}