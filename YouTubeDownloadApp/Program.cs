using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

Console.Write("Enter YouTube video URL: ");
string videoUrl = Console.ReadLine();

await DownloadVideoAsync(videoUrl);

static async Task DownloadVideoAsync(string videoUrl)
{
    var youtube = new YoutubeClient();

    // Get video info
    var video = await youtube.Videos.GetAsync(videoUrl);
    Console.WriteLine($"Downloading: {video.Title}");

    // Get available streams
    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

    //var streamInfo = streamManifest
    //.GetVideoOnlyStreams()
    //.Where(s => s.Container == Container.Mp4)
    //.GetWithHighestVideoQuality();

    // Download the stream to file
    if (streamInfo != null)
    {
        string filePath = $"{video.Title}.mp4";
        await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
        Console.WriteLine($"Video downloaded to: {filePath}");
    }
    else
    {
        Console.WriteLine("No suitable stream found.");
    }
}