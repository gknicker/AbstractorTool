using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class InvoicesController : ApiController
{
    // GET api/<controller>
    public IEnumerable<Invoice> Get()
    {
		 return Penatna.PenatnaDB.Instance.GetInvoices();
    }

    // GET api/<controller>/5
    public Invoice Get(Guid id)
    {
        return Penatna.PenatnaDB.Instance.GetInvoice(id);
    }

    // POST api/<controller>
    public int Post([FromBody]IList<Invoice> invoices)
    {
		 return Penatna.PenatnaDB.Instance.InsertInvoices(invoices);
    }

    // PUT api/<controller>/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/<controller>/5
    public void Delete(int id)
    {
    }
}
