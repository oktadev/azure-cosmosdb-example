using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using ExpenseWebApp.Models;

namespace ExpenseWebApp.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public async Task<ActionResult> Index()
		{
			ViewBag.Title = "Expenses";
			var expenses = await this.GetExpenses();

			var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			var payments = this.GetPayments(expenses, monthStart, monthStart.AddMonths(2));

			ViewBag.AlreadyPaid = this.OwedDuring(payments, monthStart, DateTime.Today - new TimeSpan(1, 0, 0, 0));
			ViewBag.DueToday = this.OwedDuring(payments, DateTime.Today, DateTime.Today);
			ViewBag.StillComing = this.OwedDuring(payments, DateTime.Today.AddDays(1), monthStart.AddMonths(1) - new TimeSpan(1, 0, 0, 0));

			ViewBag.Next1Week = this.OwedDuring(payments, DateTime.Today, DateTime.Today.AddDays(7));
			ViewBag.Next2Weeks = this.OwedDuring(payments, DateTime.Today, DateTime.Today.AddDays(14));
			ViewBag.Next1Month = this.OwedDuring(payments, DateTime.Today, DateTime.Today.AddMonths(1));
			ViewBag.Next2Months = this.OwedDuring(payments, DateTime.Today, DateTime.Today.AddMonths(2));

			return View(expenses);
		}

		[HttpPost]
		[ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateAsync([Bind(Include = "Name,Amount,Frequency,StartDate")] Expense input)
		{
			if (ModelState.IsValid)
				using (var client = new DocumentClient(new Uri(MvcApplication.CosmosConfig.EndPointUrl), MvcApplication.CosmosConfig.AuthorizationKey))
				{
					await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(MvcApplication.CosmosConfig.DatabaseId, MvcApplication.CosmosConfig.ExpenseCollectionId), input);
					return RedirectToAction("Index");
				}

			return View(input);
		}


		private List<Payment> GetPayments(List<Expense> expenses, DateTime start, DateTime finish)
		{
			var ret = new List<Payment>();

			//get all payments between specified start and finish dates
			foreach (var e in expenses)
				for (var x = e.StartDate; x <= finish;)
				{
					if (x >= start)
						ret.Add(new Payment() { Amount = e.Amount, DueDate = x });
					x = this.GetNextDueDate(x, e.Frequency);
				}

			return ret;
		}
		private DateTime GetNextDueDate(DateTime lastDueDate, string frequency)
		{
			if (frequency == "w")
				return lastDueDate.AddDays(7);
			else if (frequency == "m")
				return lastDueDate.AddMonths(1);
			else if (frequency == "q")
				return lastDueDate.AddMonths(3);
			else if (frequency == "y")
				return lastDueDate.AddYears(1);

			throw new Exception("Invalid expense frequency - unable to get the next due date");
		}
		private double OwedDuring(List<Payment> payments, DateTime start, DateTime finish)
		{
			double ret = 0;

			for (var i = 0; i < payments.Count; i++)
				if (payments[i].DueDate >= start && payments[i].DueDate <= finish)
					ret += payments[i].Amount;

			return ret;
		}

		private async Task<List<Expense>> GetExpenses()
		{
			var ret = new List<Expense>();

			using (var client = new DocumentClient(new Uri(MvcApplication.CosmosConfig.EndPointUrl), MvcApplication.CosmosConfig.AuthorizationKey))
			{
				IDocumentQuery<Expense> query = client.CreateDocumentQuery<Expense>(
					UriFactory.CreateDocumentCollectionUri(MvcApplication.CosmosConfig.DatabaseId, MvcApplication.CosmosConfig.ExpenseCollectionId),
					new FeedOptions { MaxItemCount = -1 })
					.AsDocumentQuery();

				while (query.HasMoreResults)
					ret.AddRange(await query.ExecuteNextAsync<Expense>());

				for (var i = 0; i < ret.Count; i++)
					ret[i].MonthlyCost = CalculateMonthlyCost(ret[i]);
			}

			return ret;
		}
		private double CalculateMonthlyCost(Expense input)
		{
			if (input.Frequency == "w")
				return input.Amount * 52 / 12;

			else if (input.Frequency == "m")
				return input.Amount;

			else if (input.Frequency == "q")
				return input.Amount / 3;

			else if (input.Frequency == "y")
				return input.Amount / 12;

			throw new Exception("Invalid expense frequency - unable to calculate the monthly cost");
		}
	}
}