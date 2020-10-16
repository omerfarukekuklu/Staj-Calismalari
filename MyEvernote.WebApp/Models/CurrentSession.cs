using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace MyEvernote.WebApp.Models
{
    public class CurrentSession
    {
        public static EvernoteUser User
        {
            get
            {
                return Get<EvernoteUser>("login");
            }
        }

        public static void Set<T>(string key, T obj)
        {
            HttpContext.Current.Session[key] = obj;
        }

        public static T Get<T>(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }

        public static void Remove(string key)
        {
            if (Get<EvernoteUser>(key) != null)
            {
                HttpContext.Current.Session.Remove(key);
            }
        }

        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}