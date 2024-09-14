using System.Text.Json.Serialization;

namespace Sunrise.Server.API.Serializable.Response;

public class StatusResponse(int usersOnline, long totalUsers)
{
    [JsonPropertyName("is_online")]
    public bool IsOnline { get; set; } = true; // If we are here, the server is online. Makes sense, right?

    [JsonPropertyName("users_online")]
    public int UsersOnline { get; set; } = usersOnline;

    [JsonPropertyName("total_users")]
    public long TotalUsers { get; set; } = totalUsers;

}