using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace TouchColors.Helper
{
    public class SpeechHelper : ISpeechHelper
    {
        private readonly SpeechSynthesizer _synth;
        private readonly MediaElement _mediaElement;
        private readonly SpeechRecognizer _speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> _recognitionOperation;

        public SpeechHelper()
        {
            _mediaElement = new MediaElement();
            _synth = new SpeechSynthesizer();
            _synth.Voice = SpeechSynthesizer.DefaultVoice; //SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
            _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        }

        public async Task<bool> InitializeSpeech(IEnumerable<string> responses)
        {
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();

            if (permissionGained)
            {
                var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
                _speechRecognizer.Constraints.Add(listConstraint);
                await _speechRecognizer.CompileConstraintsAsync();
            }

            return permissionGained;
        }

        public async Task Speak(string text)
        {
            using (var stream = await _synth.SynthesizeTextToStreamAsync(text))
            using (var br = new BinaryReader(stream.AsStreamForRead()))
            {
                var duration = GetDuration(br);

                _mediaElement.SetSource(stream, stream.ContentType);
                _mediaElement.Play();

                await Task.Delay(duration); //Delay to avoid speech recognizer to start too soon.
            }
        }

        private int GetDuration(BinaryReader br)
        {
            var riff = new string(br.ReadChars(4));
            var dwFileLength = br.ReadUInt32() + 8;
            var sRiffType = new string(br.ReadChars(4));

            var fmt = new string(br.ReadChars(4));
            var fmtChunkSize = br.ReadUInt32();
            var wFormatTag = br.ReadUInt16();
            var wChannels = br.ReadUInt16();
            var dwSamplesPerSec = br.ReadUInt32();
            var dwAvgBytesPerSec = br.ReadUInt32();
            var wBlockAlign = br.ReadUInt16();
            var dwBitsPerSample = br.ReadUInt32();

            var data = new string(br.ReadChars(4));
            var dataChunkSize = br.ReadUInt32();

            var duration = (dataChunkSize / (double)dwAvgBytesPerSec)*1000;

            return (int)duration;
            //if (br.ReadInt32() < 16) //formatChunkLength
            //    throw new InvalidDataException("Invalid WaveFormat Structure");

            //br.ReadUInt16(); //WaveFormatEncoding
            //br.ReadInt16(); //channels
            //br.ReadInt32(); //sampleRate
            //var averageBytesPerSecond = (double)br.ReadInt32();

           // return (int)((br.BaseStream.Length * 8 * 1024 * 1000) / averageBytesPerSecond);
        }


        private uint CalculateWaveLength(string FileName)
        {
            string mmioinfo;
            string mmckinfoRIFF;
            string mmckinfoFMT;
            string mmckinfoDATA;
            string mmr;

            WAVEFORMATEXTENSIBLE waveFormat = new WAVEFORMATEXTENSIBLE();
            HMMIO mmh = mmioOpen(FileName, mmioinfo, MMIO_DENYNONE | MMIO_READ);

            mmr = mmioDescend(mmh, mmckinfoRIFF, null, 0);
            if (mmr != MMSYSERR_NOERROR && mmckinfoRIFF.ckid != FOURCC_RIFF)
            {
                Console.Write("Unable to find RIFF section in .WAV file, possible file format error: {0:x}\n", mmr);
                Environment.Exit(1);
            }
            if (mmckinfoRIFF.fccType != mmioFOURCC('W', 'A', 'V', 'E'))
            {
                Console.Write("RIFF file {0} is not a WAVE file, possible file format error\n", FileName);
                Environment.Exit(1);
            }

            // It's a wave file, read the format tag.
            mmckinfoFMT.ckid = mmioFOURCC('f', 'm', 't', ' ');
            mmr = mmioDescend(mmh, mmckinfoFMT, mmckinfoRIFF, MMIO_FINDCHUNK);
            if (mmr != MMSYSERR_NOERROR)
            {
                Console.Write("Unable to find FMT section in RIFF file, possible file format error: {0:x}\n", mmr);
                Environment.Exit(1);
            }
            // The format tag fits into a WAVEFORMAT, so read it in.
            if (mmckinfoFMT.cksize >= sizeof(WAVEFORMAT))
            {
                // Read the requested size (limit the read to the existing buffer though).
                int readLength = mmckinfoFMT.cksize;
                if (mmckinfoFMT.cksize >= sizeof(WAVEFORMATEXTENSIBLE))
                {
                    readLength = sizeof(WAVEFORMATEXTENSIBLE);
                }
                if (readLength != mmioRead(mmh, (string)waveFormat, readLength))
                {
                    Console.Write("Read error reading WAVE format from file\n");
                    Environment.Exit(1);
                }
            }
            if (waveFormat.Format.wFormatTag != WAVE_FORMAT_PCM)
            {
                Console.Write("WAVE file {0} is not a PCM format file, it's a {1:D} format file\n", FileName, waveFormat.Format.wFormatTag);
                Environment.Exit(1);
            }
            // Pop back up a level
            mmr = mmioAscend(mmh, mmckinfoFMT, 0);
            if (mmr != MMSYSERR_NOERROR)
            {
                Console.Write("Unable to pop up in RIFF file, possible file format error: {0:x}\n", mmr);
                Environment.Exit(1);
            }

            // Now read the data section.
            mmckinfoDATA.ckid = mmioFOURCC('d', 'a', 't', 'a');
            mmr = mmioDescend(mmh, mmckinfoDATA, mmckinfoRIFF, MMIO_FINDCHUNK);
            if (mmr != MMSYSERR_NOERROR)
            {
                Console.Write("Unable to find FMT section in RIFF file, possible file format error: {0:x}\n", mmr);
                Environment.Exit(1);
            }
            // Close the handle, we're done.
            mmr = mmioClose(mmh, 0);
            //
            // We now have all the info we need to calculate the file size. Use 64bit math 
            // to avoid potential rounding issues.
            //
            long fileLengthinMS = mmckinfoDATA.cksize * 1000;
            fileLengthinMS /= waveFormat.Format.nAvgBytesPerSec;
            return fileLengthinMS;
        }


        public async Task<string> Recognize()
        {
            if (_speechRecognizer.State != SpeechRecognizerState.Idle && _recognitionOperation != null)
            {
                _recognitionOperation.Cancel();
                _recognitionOperation = null;
                return null;
            }

            try
            {
                _recognitionOperation = _speechRecognizer.RecognizeAsync();
                var result = await _recognitionOperation;
                return result.Text;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}