using System;

namespace ExpenseWebApp.Models
{
	public class CosmosConfig
	{
		public string EndPointUrl { get; set; }
		public string AuthorizationKey { get; set; }

		public string DatabaseId { get; set; }
		public string ExpenseCollectionId { get; set; }
	}
}