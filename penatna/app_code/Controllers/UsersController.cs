using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class UsersController : ApiController
{
    // GET api/<controller>
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<controller>/5
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<controller>
    public void Post([FromBody]User user)
    {
		 Penatna.PenatnaDB.Instance.InsertUser(user);
    }

	 public void Post(Guid id, string propChanged, string newValue)
	 {
		 Penatna.PenatnaDB.Instance.UpdateUser(id, propChanged, newValue);
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
