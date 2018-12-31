<%@ Application Language="C#" %>
<%@ Import Namespace="Penatna" %>
<%@ Import Namespace="System.Net.Http.Formatting" %>
<%@ Import Namespace="System.Net.Http.Headers" %>
<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
		 HttpConfiguration config = GlobalConfiguration.Configuration;
		 config.Routes.MapHttpRoute(
			  name: "API Default",
			  routeTemplate: "api/{controller}/{id}",
			  defaults: new { id = System.Web.Http.RouteParameter.Optional }
		 );
		 JsonMediaTypeFormatter formatter = config.Formatters.JsonFormatter;
		 formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
		 formatter.MediaTypeMappings.Add(new QueryStringMapping("json", "true", "application/json"));
		 formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		 formatter.SerializerSettings.Formatting = Formatting.Indented;
	 }

</script>
