using Google.Apis.Services;
using Google.Apis.YouTube.v3;

Console.Write("Enter search term: ");
string searchTerm = Console.ReadLine();

await SearchYouTubeAsync(searchTerm);

static async Task SearchYouTubeAsync(string searchTerm)
{
    var youtubeService = new YouTubeService(new BaseClientService.Initializer()
    {
        ApiKey = "API_Key", // Replace with your API key
        ApplicationName = "YouTubeSearchApp"
    });

    var searchRequest = youtubeService.Search.List("snippet");
    searchRequest.Q = searchTerm;
    searchRequest.Type = "video";
    searchRequest.MaxResults = 5;

    var searchResponse = await searchRequest.ExecuteAsync();

    Console.WriteLine($"Search results for '{searchTerm}':");
    foreach (var result in searchResponse.Items)
    {
        Console.WriteLine($"Title: {result.Snippet.Title}");
        Console.WriteLine($"Description: {result.Snippet.Description}");
        Console.WriteLine($"URL: https://www.youtube.com/watch?v={result.Id.VideoId}");
        Console.WriteLine();
    }
}