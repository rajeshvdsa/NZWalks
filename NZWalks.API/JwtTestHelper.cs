using System.Net.Http.Headers;

// This is a quick test to verify JWT authentication
// Run this in Program.cs or as a separate test

public class JwtTestHelper
{
    public static async Task TestJwtAuthentication()
    {
        var httpClient = new HttpClient();

        // 1. Login and get token
        var loginRequest = new { username = "rannarap", password = "your_password" };
        var loginJson = System.Text.Json.JsonSerializer.Serialize(loginRequest);
        var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");

        var loginResponse = await httpClient.PostAsync("http://localhost:5029/api/auth/login", loginContent);
        var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

        Console.WriteLine($"Login Response Status: {loginResponse.StatusCode}");
        Console.WriteLine($"Login Response Body: {loginResponseBody}");

        if (!loginResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Login failed!");
            return;
        }

        // Parse token from response
        var jsonDoc = System.Text.Json.JsonDocument.Parse(loginResponseBody);
        var token = jsonDoc.RootElement.GetProperty("token").GetString();

        Console.WriteLine($"\nToken: {token}");

        // 2. Use token to access protected endpoint
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var regionsResponse = await httpClient.GetAsync("http://localhost:5029/api/regions");
        var regionsBody = await regionsResponse.Content.ReadAsStringAsync();

        Console.WriteLine($"\nRegions Response Status: {regionsResponse.StatusCode}");
        Console.WriteLine($"Regions Response Body: {regionsBody}");

        if (regionsResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("\n✅ JWT Authentication is working!");
        }
        else
        {
            Console.WriteLine("\n❌ JWT Authentication failed!");
        }
    }
}
