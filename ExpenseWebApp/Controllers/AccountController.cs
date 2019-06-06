using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Okta.AspNet;

namespace ExpenseWebApp.Controllers
{
	public class AccountController : Controller
	{
		public ActionResult Login()
		{
			if (!HttpContext.User.Identity.IsAuthenticated)
			{
				HttpContext.GetOwinContext().Authentication.Challenge(
					OktaDefaults.MvcAuthenticationType);
				return new HttpUnauthorizedResult();
			}

			return RedirectToAction("Index", "Expense");
		}

		[HttpPost]
		public ActionResult Logout()
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				HttpContext.GetOwinContext().Authentication.SignOut(
					CookieAuthenticationDefaults.AuthenticationType,
					OktaDefaults.MvcAuthenticationType);
			}

			return RedirectToAction("Index", "Home");
		}

		public ActionResult PostLogout()
		{
			return RedirectToAction("Index", "Home");
		}
	}
}