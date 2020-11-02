using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Identity
{
   [Area("Identity")]
   public class ValidationController : Controller
   {
      private UserManager<IdentityUser> _userManager;
      public ValidationController(UserManager<IdentityUser> userManager)
      {
         _userManager = userManager;
      }

      public async Task<IActionResult> CheckEmail([Bind(Prefix = "Input.Email")] string email = "")
      {
         var user = await _userManager.FindByEmailAsync(email);
         if (user == null)
            return Json(true);
         else
            return Json("This Email already exists");
      }
   }
}
