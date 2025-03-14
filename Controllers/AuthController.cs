using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly string tenant_id = "39685215-f7d9-487b-8330-d95f44f9ef1e";
        private readonly string webappcallswebapi_application_client_id = "0b0545df-455d-46ed-afab-b51cb05d1853";
        private readonly string web_API_application_client_id = "95e9e6f2-ffea-46fa-94b4-58dc43ffaefe";

        private readonly string webapptenant_id = "39685215-f7d9-487b-8330-d95f44f9ef1e";
        [HttpGet]
        public ActionResult AuthCode()
        {
            string authCodeURI = $"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/authorize?client_id={webappcallswebapi_application_client_id}&response_type=code&redirect_uri=http://localhost&response_mode=query&scope=api://{web_API_application_client_id}/Files.Read";

            return Ok(authCodeURI);
        }
        [HttpPost]
        [Route("{authorization_code}")]
        public async Task<ActionResult> SignIn(string authorization_code)
        {
            var client = new HttpClient();
            string client_secret = Environment.GetEnvironmentVariable("WEBAPPSECRET");
            if (string.IsNullOrEmpty(client_secret))
            {
                return BadRequest("Client secret not loaded");
            }

            var tokenEndpoint = $"https://login.microsoftonline.com/{webapptenant_id}/oauth2/v2.0/token";

            // Create proper form data as shown in the curl example
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = webappcallswebapi_application_client_id,
                ["scope"] = $"api://{web_API_application_client_id}/Files.Read",
                ["code"] = authorization_code,
                ["session_state"] = webappcallswebapi_application_client_id,
                ["redirect_uri"] = "http://localhost",
                ["grant_type"] = "authorization_code",
                ["client_secret"] = client_secret
            });

            var response = await client.PostAsync(tokenEndpoint, formContent);

            var responseString = await response.Content.ReadAsStringAsync();

            return Ok(responseString);
        }
    }
}
