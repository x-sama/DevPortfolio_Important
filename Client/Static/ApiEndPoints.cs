namespace Client.Static;

internal static class ApiEndPoints
{
    #if DEBUG
    //when under developement Producing
    internal const string  ServerBaseUrl = "https://localhost:5003";
    #else
    //when production state
    internal const string  ServerBaseUrl = "https://valravnserver.azurewebsites.net";
    #endif

    internal static readonly string SCategories = $"{ServerBaseUrl}/api/categories";
}