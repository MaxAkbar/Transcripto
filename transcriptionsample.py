import whisper
import warnings

from whisper.utils import get_writer

warnings.filterwarnings('ignore')

model = whisper.load_model("turbo")
result = model.transcribe("intro.wav")

writer = get_writer("srt", ".") 
writer(result, "intro.wav", {"max_line_width":55, "max_line_count":2, "highlight_words":False} )

print(result["text"])