using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using SBS_Ecommerce.Framework.Configurations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SBS_Ecommerce.Models.Extension
{
    public class CustomUserValidator<TUser> : UserValidator<TUser, string> where TUser : ApplicationUser
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="manager"></param>
        public CustomUserValidator(UserManager<TUser, string> manager) : base(manager)
        {
            this.Manager = manager;
        }

        private UserManager<TUser, string> Manager { get; set; }

        /// <summary>
        ///     Validates a user before saving
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<IdentityResult> ValidateAsync(TUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();
            if (RequireUniqueEmail)
            {
                await ValidateEmail(item, errors);
                await ValidateUserName(item, errors);
            }
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }

        private async Task ValidateUserName(TUser user, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, SBSMessages.MessageIncorrectLogin, "Name"));
            }
            else if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, @"^[A-Za-z0-9@_\.]+$"))
            {
                // If any characters are not letters or digits, its an illegal user name
                errors.Add(String.Format(CultureInfo.CurrentCulture, SBSMessages.InvalidUserName, user.UserName));
            }
            else
            {
                var owner = await Manager.FindByNameAsync(user.UserName);
                if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
                {
                    errors.Add(String.Format(CultureInfo.CurrentCulture, SBSMessages.DuplicateName, user.UserName));
                }
            }
        }

        // make sure email is not empty, valid, and unique
        private async Task ValidateEmail(TUser user, List<string> errors)
        {
            if (!user.Email.IsNullOrWhiteSpace())
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    errors.Add(String.Format(CultureInfo.CurrentCulture, SBSMessages.MessageIncorrectLogin, "Email"));
                    return;
                }
                try
                {
                    var m = new MailAddress(user.Email);
                }
                catch (FormatException)
                {
                    errors.Add(String.Format(CultureInfo.CurrentCulture, SBSMessages.InvalidEmail, "Email"));
                    return;
                }
            }
            var owner = await Manager.FindByEmailAsync(user.Email);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, SBSMessages.DuplicateEmail, "Email"));
            }
        }
    }
}