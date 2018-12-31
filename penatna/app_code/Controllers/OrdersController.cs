using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Penatna
{
	public class OrdersController : ApiController
	{
		// GET api/<controller>
		public IEnumerable<Order> Get()
		{
			return Penatna.PenatnaDB.Instance.GetOrders();
		}

		// GET api/<controller>/5
		public Order Get(Guid id)
		{
			return Penatna.PenatnaDB.Instance.GetOrder(id);
		}

		// POST api/<controller>
		public void Post([FromBody]IList<Order> orders)
		{
			Penatna.PenatnaDB.Instance.InsertOrders(orders);
		}

		// PUT api/<controller>/5
		public void Put(Guid id, [FromBody]Order value)
		{
		}

		// DELETE api/<controller>/5
		public void Delete(Guid id)
		{
		}
	}
}
