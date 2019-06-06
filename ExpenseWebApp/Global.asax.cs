using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;

using ExpenseWebApp.Models;

namespace ExpenseWebApp
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			MvcApplication.InitCosmosConfig();
		}

		public static CosmosConfig CosmosConfig { get; set; }

		private static void InitCosmosConfig()
		{
			AntiForgeryConfig.UniqueClaimTypeIdentifier = "name";
			MvcApplication.CosmosConfig = new CosmosConfig()
			{
				DatabaseId = ConfigurationManager.AppSettings["Cosmos.DatabaseId"],
				EndPointUrl = ConfigurationManager.AppSettings["Cosmos.EndPointUrl"],
				AuthorizationKey = ConfigurationManager.AppSettings["Cosmos.AuthorizationKey"],
				ExpenseCollectionId = ConfigurationManager.AppSettings["Cosmos.ExpenseCollectionId"]
			};
		}
	}
}
