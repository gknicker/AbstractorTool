using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class AbstractsController : ApiController
{
    // GET api/<controller>
    public IEnumerable<Abstract> Get()
    {
        return Penatna.PenatnaDB.Instance.GetAbstracts();
    }

    // GET api/<controller>/5
    public Abstract Get(Guid id)
    {
        return Penatna.PenatnaDB.Instance.GetAbstract(id);
    }

    // POST api/<controller>
    public void Post([FromBody]IList<Abstract> abstracts)
    {
		 Penatna.PenatnaDB.Instance.InsertAbstracts(abstracts);
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
