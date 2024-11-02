using Python.Runtime;

Environment.SetEnvironmentVariable("PYTHONHOME", @"C:\Users\maxim\AppData\Local\Programs\Python\Python311\");
Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"C:\Users\maxim\AppData\Local\Programs\Python\Python311\python311.dll");
// Enable legacy BinaryFormatter serialization support
AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);

// Initialize the Python engine
PythonEngine.Initialize();

using (Py.GIL())
{
    // Import your Python script here (e.g., "whisper.py")
    dynamic whisperModule = Py.Import("whisper");
    dynamic whisperUtils = Py.Import("whisper.utils");
    dynamic warnings = Py.Import("warnings");

    warnings.filterwarnings("ignore");

    // Get the get_writer function from the utils module
    dynamic get_writer = whisperUtils.GetAttr("get_writer");
    dynamic whisperModel = whisperModule.load_model("large-v3");

    // Use Python to transcribe audio
    string audioPath = "4 Steps to accelerate your AI Journey.mp4";
    dynamic result = whisperModel.transcribe(audioPath);

    // Create a writer for SRT files using the get_writer function
    dynamic writer = get_writer("srt", ".");

    // Define the options dictionary for the writer
    using (PyDict options = new PyDict())
    {
        options.SetItem("max_line_width", 55.ToPython());
        options.SetItem("max_line_count", 2.ToPython());
        options.SetItem("highlight_words", false.ToPython());

        // Write the subtitles using the writer function
        writer(result, audioPath, options);
    }

    dynamic dict = result["segments"];

    foreach (var segment in dict)
    {
        // Accessing the segment fields
        dynamic id = segment["id"];
        dynamic seek = segment["seek"];
        dynamic startTime = segment["start"];
        dynamic endTime = segment["end"];
        dynamic text = segment["text"];
        dynamic tokens = segment["tokens"];

        // Convert to string to use in C#
        Console.WriteLine($"Start: {startTime}, End: {endTime}, Text: {text}");
    }

    Console.WriteLine(result["text"]);

    // Manually release the Python object references
    whisperModel.Dispose();
}

// Shut down the Python engine
PythonEngine.Shutdown();

