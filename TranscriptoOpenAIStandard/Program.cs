using System.Text.Json;
using System.Text;
using OpenAI.Audio;
using OpenAI;
using System.ClientModel;

var openAIClientOptions = new OpenAIClientOptions
{
    Endpoint = new Uri("http://127.0.0.1:8000/v1")
};
var apiKeyCredential = new ApiKeyCredential("FAKE_API_KEY");
var client = new AudioClient("whisper", apiKeyCredential, openAIClientOptions);
var audioFilePath = "4 Steps to accelerate your AI Journey.mp4";
var transcription = client.TranscribeAudio(audioFilePath);

Console.WriteLine($"{transcription.Value.Text}");