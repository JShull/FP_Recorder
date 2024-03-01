using System;
using UnityEngine;
using UnityEditor.Recorder;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "Audio", menuName = "FuzzPhyte/Recorder/Type/AudioType")]
    [Serializable]
    public class AudioRecorder : FP_OutputFormatSO
    {
        [Space]
        public AudioRecorderSettings AudioRecorderSettings;
        public UnityEditor.Recorder.Input.AudioInputSettings AudioInputSettings;
        public UnityEditor.Recorder.Input.AudioInput AudioInput;
        //public WAVEncoder AudioRecorderWavSettings;
        //public AudioCompressionFormat FormatAudio;
        public void AudioSetupFormat()
        {
            //formatwav
        }
    }
}
