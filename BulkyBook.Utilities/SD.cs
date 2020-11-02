using BulkyBook.DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.Utilities
{
   public static class SD
   {
      public const string Role_UserIndi = "Individual Customer";
      public const string Role_UserComp = "Company Customer";
      public const string Role_Admin = "Admin";
      public const string Role_Employee = "Employee";

      public const string StatusPending = "Pending";
      public const string StatusApproved = "Approved";
      public const string StatusInProcess = "Processing";
      public const string StatusShipped = "Shipped";
      public const string StatusCancelled = "Cancelled";
      public const string StatusRefunded = "Refunded";

      public const string PaymentStatusPending = "Pending";
      public const string PaymentStatusApproved = "Approved";
      public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
      public const string PaymentStatusRejected = "Rejected";


      public const string Shopping_Cart = "Shopping Cart Session"; 

      public static double CalcPrice(int count,double price,double price50,double price100)
      {
         if (count < 50)
            return price;
         if (50 < count && count < 100)
            return price50;
         else
            return price100;
      }
   }
}
