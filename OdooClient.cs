using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotnetOdooCli
{
    public class OdooClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly string _db;
        private readonly string _user;
        private readonly string _password;

        public OdooClient(string url, string db, string user, string password)
        {
            _url = url;
            _db = db;
            _user = user;
            _password = password;
            _httpClient = new HttpClient();
        }

        public async Task<int?> AuthenticateAsync()
        {
            string commonEndpoint = $"{_url.TrimEnd('/')}/xmlrpc/2/common";
            
            string xmlPayload = $@"<?xml version=""1.0""?>
<methodCall>
  <methodName>authenticate</methodName>
  <params>
    <param><value><string>{_db}</string></value></param>
    <param><value><string>{_user}</string></value></param>
    <param><value><string>{_password}</string></value></param>
    <param><value><struct></struct></value></param>
  </params>
</methodCall>";

            var content = new StringContent(xmlPayload, Encoding.UTF8, "text/xml");

            try
            {
                var response = await _httpClient.PostAsync(commonEndpoint, content);
                response.EnsureSuccessStatusCode();

                var responseXml = await response.Content.ReadAsStringAsync();
                var xdoc = XDocument.Parse(responseXml);

                // Odoo returns an int if successful, or a boolean false if auth fails.
                var valueElement = xdoc.Descendants("value").FirstOrDefault();
                if (valueElement != null)
                {
                    var intElement = valueElement.Element("int") ?? valueElement.Element("i4");
                    if (intElement != null && int.TryParse(intElement.Value, out int uid))
                    {
                        return uid;
                    }
                }
                
                return null; // Auth failed or invalid response
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during Odoo connection: {ex.Message}", ex);
            }
        }
    }
}
