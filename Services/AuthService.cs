using System.Text.Json;
using Project1.DTOs;
using Project1.ErrorHandling;

namespace Project1.Services
{
    public class AuthService
    {
        private readonly string tenant_id = "39685215-f7d9-487b-8330-d95f44f9ef1e";
        private readonly string webappcallswebapi_application_client_id = "0b0545df-455d-46ed-afab-b51cb05d1853";
        private readonly string web_API_application_client_id = "95e9e6f2-ffea-46fa-94b4-58dc43ffaefe";
        private readonly string webapptenant_id = "39685215-f7d9-487b-8330-d95f44f9ef1e";

        private readonly string redirect_uri = "http://localhost";
        public string GetAuthCodeURI()
        {
            string authCodeURI = $"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/authorize?client_id={webappcallswebapi_application_client_id}&response_type=code&redirect_uri={redirect_uri}&response_mode=query&scope=api://{web_API_application_client_id}/Files.Read";

            return authCodeURI;
        }

        public async Task<Result<AuthResponseDTO>> GetBearerToken(string authorization_code)
        {

            var client = new HttpClient();
            string client_secret = Environment.GetEnvironmentVariable("WEBAPPSECRET");
            if (string.IsNullOrEmpty(client_secret))
            {
                return Result<AuthResponseDTO>.Failure("Client secret is empty");
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

            HttpResponseMessage response = await client.PostAsync(tokenEndpoint, formContent);

            string responseString = await response.Content.ReadAsStringAsync();

            AuthResponseDTO authResponseDTO = JsonSerializer.Deserialize<AuthResponseDTO>(responseString);

            return Result<AuthResponseDTO>.Success(authResponseDTO);
        }

    }
}