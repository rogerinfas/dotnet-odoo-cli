using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace OdooDotNetClient
{
    public class OdooClient : IOdooClient
    {
        private readonly HttpClient _httpClient;
        private readonly OdooOptions _options;

        public OdooClient(HttpClient httpClient, IOptions<OdooOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<int?> AuthenticateAsync()
        {
            string commonEndpoint = $"{_options.Url.TrimEnd('/')}/xmlrpc/2/common";
            
            string xmlPayload = $@"<?xml version=""1.0""?>
<methodCall>
  <methodName>authenticate</methodName>
  <params>
    <param><value><string>{_options.Db}</string></value></param>
    <param><value><string>{_options.User}</string></value></param>
    <param><value><string>{_options.Password}</string></value></param>
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

                var valueElement = xdoc.Descendants("value").FirstOrDefault();
                if (valueElement != null)
                {
                    var intElement = valueElement.Element("int") ?? valueElement.Element("i4");
                    if (intElement != null && int.TryParse(intElement.Value, out int uid))
                    {
                        return uid;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during Odoo connection: {ex.Message}", ex);
            }
        }
    }
}
