using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penatna
{
	public class PenatnaDB
	{
		private static readonly PenatnaDB INSTANCE = new PenatnaDB();
		public static PenatnaDB Instance { get { return INSTANCE; } }

		#region user methods
		public User GetUser(string name, string pwd)
		{
			return new PenatnaDataContext().Users.SingleOrDefault(a => a.LoginName == name && a.Password == pwd);
		}

		public void InsertUser(User user)
		{
			using (PenatnaDataContext context = new PenatnaDataContext()) {
				if (!context.Users.Any(a => a.LoginName == user.LoginName) && !context.Users.Any(a => a.Email == user.Email)) {
					user.DateCreated = DateTime.Now;
					user.DateUpdated = DateTime.Now;
					context.Users.InsertOnSubmit(user);
				}
			}
		}

		public void UpdateUser(Guid id, string propertyChanged, string newValue)
		{
			using (PenatnaDataContext db = new PenatnaDataContext()) {
				User dbUser = db.Users.SingleOrDefault(a => a.Id == id);
				dbUser.DateUpdated = DateTime.Now;
				switch (propertyChanged) {
					case "LoginName":
						dbUser.LoginName = newValue;
						break;
					case "Password":
						dbUser.Password = newValue;
						break;
					case "Email":
						dbUser.Email = newValue;
						break;
					case "InvoicePrefix":
						dbUser.InvoicePrefix = newValue;
						break;
					case "NextInvoiceNumber":
						int nextInv = Convert.ToInt32(newValue);
						IQueryable<int> query = from invoice in db.Invoices
														join order in db.Orders on invoice.OrderId equals order.Id
														where order.UserId == id
														orderby invoice.Number descending
														select invoice.Number;
						int maxUsedNumber = query.FirstOrDefault();
						if (nextInv <= maxUsedNumber) {
							nextInv = maxUsedNumber + 1;
						}
						dbUser.NextInvoiceNumber = nextInv;
						break;
					case "SearchFee":
						dbUser.SearchFee = Convert.ToDecimal(newValue);
						break;
					case "CopyFee":
						dbUser.CopyFee = Convert.ToDecimal(newValue);
						break;
					case "FirstName":
						dbUser.FirstName = newValue;
						break;
					case "MiddleInitial":
						dbUser.MiddleInitial = Convert.ToChar(newValue.Substring(0, 1));
						break;
					case "LastName":
						dbUser.LastName = newValue;
						break;
					case "Address":
						dbUser.Address = newValue;
						break;
					case "City":
						dbUser.City = newValue;
						break;
					case "StateAbbr":
						if (newValue.Length > 2) { dbUser.StateAbbr = newValue.Substring(0, 2);
						} else { dbUser.StateAbbr = newValue; }
						break;
					case "Zip":
						if (newValue.Length == 5) { dbUser.Zip = Convert.ToDecimal(newValue); }
						break;
					case "ZipPlus4":
						dbUser.ZipPlus4 = Convert.ToDecimal(newValue);
						break;
					case "Phone":
						dbUser.Phone = newValue;
						break;
					default:
						throw new Exception("Unable to change field!");
				}
			}
		}
		#endregion

		#region order methods
		public Order GetOrder(Guid id)
		{
			return new PenatnaDataContext().Orders.SingleOrDefault(a => a.Id == id);
		}

		public IEnumerable<Order> GetOrders()
		{
			return new PenatnaDataContext().Orders;
		}

		public int GetPageCount(Guid orderId)
		{
			using (PenatnaDataContext context = new PenatnaDataContext()) {
				return new PenatnaDataContext().Pages.Count(a => a.OrderId == orderId);
			}
		}

		public int InsertOrders(IList<Order> orders)
		{
			int createCount = 0;
			using (PenatnaDataContext context = new PenatnaDataContext()) {
				foreach (Order order in orders) {
					if (!context.Orders.Any(a => a.TaskID == order.TaskID)) {
						order.Id = Guid.NewGuid();
						order.DateImported = DateTime.Now;
						context.Orders.InsertOnSubmit(order);
						createCount++;
					}
				}
				context.SubmitChanges();
			}
			return createCount;
		}
		#endregion

		#region abstract methods
		public Abstract GetAbstract(Guid id)
		{
			return new PenatnaDataContext().Abstracts.SingleOrDefault(a => a.Id == id);
		}

		public IEnumerable<Abstract> GetAbstracts()
		{
			return new PenatnaDataContext().Abstracts;
		}

		public int InsertAbstracts(IList<Abstract> abstracts)
		{
			int createCount = 0;
			using (PenatnaDataContext context = new PenatnaDataContext()) {
				foreach (Abstract abs in abstracts) {
					if (!context.Abstracts.Any(a => a.OrderId == abs.OrderId)) {
						abs.Id = Guid.NewGuid();
						abs.LastUpdated = DateTime.Now;
						context.Abstracts.InsertOnSubmit(abs);
						createCount++;
					}
				}
				context.SubmitChanges();
			}
			return createCount;
		}
		#endregion

		#region page methods
		public IEnumerable<Page> GetPagesByOrder(Guid orderId)
		{
			return new PenatnaDataContext().Pages.Where(a => a.OrderId == orderId).OrderBy(a => a.PageNumber);
		}
		#endregion

		#region invoice methods
		public Invoice GetInvoice(Guid id)
		{
			return new PenatnaDataContext().Invoices.SingleOrDefault(a => a.Id == id);
		}

		public IEnumerable<Invoice> GetInvoices()
		{
			return new PenatnaDataContext().Invoices;
		}

		public int InsertInvoices(IList<Invoice> invoices)
		{
			int createCount = 0;
			using (PenatnaDataContext context = new PenatnaDataContext()) {
				foreach (Invoice invoice in invoices) {
					if (!context.Invoices.Any(a => a.Number == invoice.Number)) {
						invoice.Id = Guid.NewGuid();
						invoice.DateCreated = DateTime.Now;
						context.Invoices.InsertOnSubmit(invoice);
						createCount++;
					}
				}
				context.SubmitChanges();
			}
			return createCount;
		}
		#endregion

	}
}
