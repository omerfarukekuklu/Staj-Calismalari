using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.WebApp.ViewModels
{
    public class OKViewModel : NotifyViewModelBase<string>
    {
        public OKViewModel()
        {
            Title = "İşlem Başarılı..";
        }
    }
}