using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook
{
   //[Route("Home/{action=index}")]
   public class HomeController : Controller
   {
      public string Index()
      {
         return "Fuck You All";
      }
   }
}
