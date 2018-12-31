using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class PagesController : ApiController
{
    // GET api/<controller>
    public void Get()
    {
    }

    // GET api/<controller>/5
    public IEnumerable<Page> Get(Guid id)
    {
        return Penatna.PenatnaDB.Instance.GetPagesByOrder(id);
    }

    // POST api/<controller>
    public void Post([FromBody]string value)
    {
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
