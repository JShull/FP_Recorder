using System;
using UnityEngine;
using UnityEditor.Recorder;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "Audio", menuName = "FuzzPhyte/Recorder/SettingsType/AudioType")]
    [Serializable]
    public class AudioRecorder : FP_OutputFormatSO
    {
        [Space]
        public AudioRecorderSettings AudioRecorderSettings;
        public FPAudioFormat Format;
        public UnityEditor.Recorder.Input.AudioInput AudioInput;
        public UnityEditor.Recorder.AudioRecorderSettings OtherAudioInput;
        public RecorderInputSettings SomeSettings;
        public UnityEditor.Recorder.Input.AudioInputSettings OtherAudioInputSettings;

        public void ReturnUnityOutputFormat()
        {
            var audioFormatSettings = new UnityEditor.Recorder.Input.AudioInput();
            var otherFormatSettings = new UnityEditor.Recorder.AudioRecorderSettings();
            //otherFormatSettings.InputsSettings
        }
    }
}
