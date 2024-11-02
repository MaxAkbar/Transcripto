import base64
import warnings
from io import BytesIO
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from whisper.utils import get_writer
import whisper
import tempfile
import os

warnings.filterwarnings('ignore')

# Initialize the FastAPI app and load the Whisper model
app = FastAPI()
model = whisper.load_model("base")  # Replace "base" with "turbo" if you have the turbo model

# Define the request and response structure
class AudioRequest(BaseModel):
    model: str
    modalities: list
    audio: dict
    messages: list

class TranscriptionResponse(BaseModel):
    transcription: str

@app.post("/v1/chat/completions", response_model=TranscriptionResponse)
async def transcribe_audio(request: AudioRequest):
    # Extract audio data from request
    try:
        input_audio = request.messages[0]['content'][1]['input_audio']['data']
        audio_format = request.messages[0]['content'][1]['input_audio']['format']
    except (KeyError, IndexError) as e:
        raise HTTPException(status_code=400, detail="Invalid request format") from e

    # Decode the Base64 audio data
    try:
        audio_data = base64.b64decode(input_audio)
    except base64.binascii.Error:
        raise HTTPException(status_code=400, detail="Invalid Base64 audio data")

    # Save the decoded audio to a temporary file
    with tempfile.NamedTemporaryFile(suffix=f".{audio_format}", delete=False) as temp_audio_file:
        temp_audio_file.write(audio_data)
        temp_audio_file_path = temp_audio_file.name

    # Transcribe the audio
    try:
        result = model.transcribe(temp_audio_file_path)
        transcription_text = result["text"]
    finally:
        # Clean up temporary file
        os.remove(temp_audio_file_path)

    # Optionally, write to an SRT file if needed
    writer = get_writer("srt", ".")
    writer(result, temp_audio_file_path, {"max_line_width": 55, "max_line_count": 2, "highlight_words": False})

    # Return the transcription in JSON format
    return TranscriptionResponse(transcription=transcription_text)
