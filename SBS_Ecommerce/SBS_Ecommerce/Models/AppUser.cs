using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SBS_Ecommerce.Models
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

            set
            {
                var CP = ClaimsPrincipal.Current.Identities.First();
                var AccountNo = CP.Claims.FirstOrDefault(p => p.Type == ClaimTypes.UserData).Value;
                CP.RemoveClaim(new Claim(ClaimTypes.UserData, AccountNo));
                CP.AddClaim(new Claim(ClaimTypes.UserData, value));
            }

        }

    }
}