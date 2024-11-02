from fastapi import FastAPI, File, Form, UploadFile
from pydantic import BaseModel
import whisper
import tempfile
import os
import warnings

warnings.filterwarnings('ignore')

# Initialize the FastAPI app and load the Whisper model
app = FastAPI()
whisper_model = whisper.load_model("base")  # Replace "base" with "turbo" if you have the turbo model

# Define the request and response structure
class TranscriptionResponse(BaseModel):
    text: str

@app.post("/v1/audio/transcriptions", response_model=TranscriptionResponse)
async def transcribe_audio(
    file: UploadFile = File(...),
    model: str = Form(...),
    language: str = Form(None),
    prompt: str = Form(None),
    response_format: str = Form(None),
    temperature: float = Form(None),
    timestamp_granularities: list[str] = Form(None)
):
    # Save the uploaded file to a temporary file
    try:
        with tempfile.NamedTemporaryFile(delete=False) as temp_audio_file:
            temp_audio_file.write(await file.read())
            temp_audio_file_path = temp_audio_file.name

        # Transcribe the audio
        result = whisper_model.transcribe(temp_audio_file_path)
        transcription_text = result["text"]
    finally:
        # Clean up temporary file
        os.remove(temp_audio_file_path)

    # Return the transcription in JSON format
    return TranscriptionResponse(text=transcription_text)
