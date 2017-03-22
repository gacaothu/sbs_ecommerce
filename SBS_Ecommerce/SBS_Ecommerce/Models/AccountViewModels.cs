﻿using SBS_Ecommerce.Framework.Configurations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SBS_Ecommerce.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = SBSConstants.Email)]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = SBSConstants.Code)]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = SBSConstants.RememberBrowser)]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = SBSConstants.Email)]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = SBSConstants.Email)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = SBSConstants.Password)]
        public string Password { get; set; }

        [Display(Name = SBSConstants.RememberMe)]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = SBSConstants.Email)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = SBSMessages.CharacterRequired, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = SBSConstants.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = SBSConstants.ConfirmPassword)]
        [Compare(SBSConstants.Password, ErrorMessage = SBSMessages.ConfirmPassword)]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = SBSConstants.Email)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = SBSMessages.CharacterRequired, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = SBSConstants.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = SBSConstants.ConfirmPassword)]
        [Compare(SBSConstants.Password, ErrorMessage = SBSMessages.ConfirmPassword)]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = SBSConstants.Email)]
        public string Email { get; set; }
    }
}
