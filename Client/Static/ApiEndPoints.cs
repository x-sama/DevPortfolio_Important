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

    internal static readonly string s_categories = $"{ServerBaseUrl}/api/categories";
    internal static readonly string s_categoriesWithPosts = $"{ServerBaseUrl}/api/categories/withposts";
    internal static readonly string s_posts = $"{ServerBaseUrl}/api/posts";
    internal static readonly string s_postsDTO = $"{ServerBaseUrl}/api/posts/dto";
    internal static readonly string SImageUpload = $"{ServerBaseUrl}/api/imageupload";
    internal static readonly string SingIn = $"{ServerBaseUrl}/api/singin";
}