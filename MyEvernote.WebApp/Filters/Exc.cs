using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Filters
{
    public class Exc : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            //Hata bilgisini TempData da sakladık.
            filterContext.Controller.TempData["LastError"] = filterContext.Exception;

            //Hatayı bizim yöneteceğimizi belirtiyoruz.
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("/Home/HasError");

        }
    }
}