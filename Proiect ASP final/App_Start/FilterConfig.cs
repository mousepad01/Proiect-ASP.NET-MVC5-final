﻿using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
