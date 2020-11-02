﻿using BulkyBook.DataLayer.Models;
using BulkyBook.DataLayer.Services.UnitOfWork;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Identity.Pages.Account
{
   [AllowAnonymous]
   public class ExternalLoginModel : PageModel
   {
      private readonly SignInManager<IdentityUser> _signInManager;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly IEmailSender _emailSender;
      private readonly ILogger<ExternalLoginModel> _logger;
      private readonly RoleManager<IdentityRole> _roleManager;
      private readonly IUnitOfWork _context;

      public ExternalLoginModel(
          SignInManager<IdentityUser> signInManager,
          UserManager<IdentityUser> userManager,
          ILogger<ExternalLoginModel> logger,
          IEmailSender emailSender,
          RoleManager<IdentityRole> roleManager,
          IUnitOfWork context)
      {
         _signInManager = signInManager;
         _userManager = userManager;
         _logger = logger;
         _emailSender = emailSender;
         _roleManager = roleManager;
         _context = context;
      }

      [BindProperty]
      public InputModel Input { get; set; }



      public string LoginProvider { get; set; }

      public string ReturnUrl { get; set; }

      [TempData]
      public string ErrorMessage { get; set; }

      public class InputModel
      {
         [Required]
         [EmailAddress]
         public string Email { get; set; }

         [Required]
         public string Name { get; set; }
         public string City { get; set; }
         public string State { get; set; }
         public string PostalCode { get; set; }
         public string PhoneNumber { get; set; }
         public int? ComId { get; set; }
         public string? Role { get; set; }
         public IEnumerable<SelectListItem>? CompanyList { get; set; }
         public IEnumerable<SelectListItem>? RoleList { get; set; }

      }

      public async Task<IActionResult> OnGetAsync()
      {
         Input = new InputModel();
         await FillList(Input);
         return RedirectToPage("./Login");
      }

      public async Task<IActionResult> OnPostAsync(string provider, string returnUrl = null)
      {
         // Request a redirect to the external login provider.
         await FillList(Input);
         ReturnUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
         var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, ReturnUrl);
         return new ChallengeResult(provider, properties);
      }

      public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
      {
         returnUrl = (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");
         if (remoteError != null)
         {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
         }
         var info = await _signInManager.GetExternalLoginInfoAsync();
         if (info == null)
         {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
         }

         // Sign in the user with this external login provider if the user already has a login.
         var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
         if (result.Succeeded)
         {
            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
            return LocalRedirect(returnUrl);
         }
         if (result.IsLockedOut)
         {
            return RedirectToPage("./Lockout");
         }
         else
         {
            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            LoginProvider = info.LoginProvider;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
               Input = new InputModel
               {
                  Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                  Name = info.Principal.FindFirstValue(ClaimTypes.Name)
               };
               await FillList(Input);
            }
            return Page();
         }
      }

      public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
      {
         await FillList(Input);
         returnUrl = (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");
         // Get the information about the user from the external login provider
         var info = await _signInManager.GetExternalLoginInfoAsync();
         if (info == null)
         {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
         }

         if (ModelState.IsValid)
         {
            var user = new MyUser
            {
               UserName = Input.Email.Split("@")[0],
               Email = Input.Email,
               City = Input.City,
               ComId = Input.ComId ?? 1,
               State = Input.State,
               PostalCode = Input.PostalCode,
               Name = Input.Name,
               PhoneNumber = Input.PhoneNumber,
               Role = Input.Role
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
               await _userManager.AddToRoleAsync(user, user.Role);
               result = await _userManager.AddLoginAsync(user, info);
               if (result.Succeeded)
               {
                  await _signInManager.SignInAsync(user, isPersistent: false);
                  _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                  //var userId = await _userManager.GetUserIdAsync(user);
                  //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                  //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                  //var callbackUrl = Url.Page(
                  //    "/Account/ConfirmEmail",
                  //    pageHandler: null,
                  //    values: new { area = "Identity", userId = userId, code = code },
                  //    protocol: Request.Scheme);

                  //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                  //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                  return LocalRedirect(returnUrl);
               }
            }
            foreach (var error in result.Errors)
            {
               ModelState.AddModelError(string.Empty, error.Description);
            }
         }

         LoginProvider = info.LoginProvider;
         ReturnUrl = returnUrl;
         return Page();
      }

      #region Method

      public async Task FillList(InputModel Input)
      {
         var _companies = await _context.Companies.GetAll();
         Input.CompanyList = _companies.Select(i => new SelectListItem()
         {
            Text = i.Name,
            Value = i.ComId.ToString()
         });
         Input.RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_UserIndi).Select(x => x.Name).Select(i => new SelectListItem
         {
            Text = i,
            Value = i
         });
      }

      #endregion
   }
}
