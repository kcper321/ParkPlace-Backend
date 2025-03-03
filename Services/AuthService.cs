
namespace ParkingAPI.Services
{
public class AuthService
    {
    public string GenerateJwtToken(string username, string password)
        {
        if (username == "testuser" && password == "password123")
            {
            var token = "generated_jwt_token";
            return token;
         }

        return null;
    }
}
}
