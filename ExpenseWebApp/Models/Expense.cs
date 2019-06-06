using System;
using Newtonsoft.Json;

namespace ExpenseWebApp.Models
{
	public class Expense
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "amount")]
		public double Amount { get; set; }

		[JsonProperty(PropertyName = "frequency")]
		public string Frequency { get; set; }

		[JsonProperty(PropertyName = "startDate")]
		public DateTime StartDate { get; set; }

		[JsonProperty(PropertyName = "monthlyCost")]
		public double MonthlyCost { get; set; }
	}
}