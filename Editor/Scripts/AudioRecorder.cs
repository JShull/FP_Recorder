using System;
using UnityEngine;
using UnityEditor.Recorder;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "Audio", menuName = "FuzzPhyte/Recorder/SettingsType/AudioType")]
    [Serializable]
    public class AudioRecorder : FP_OutputFormatSO, IRecorderOutputFormat<AudioRecorderSettings>
    {
        [Space]
        //public AudioRecorderSettings AudioRecorderSettings;
        public FPAudioFormat Format;
        

        public AudioRecorderSettings ReturnUnityOutputFormat(GameObject gObject=null)
        {

            var otherFormatSettings = new UnityEditor.Recorder.AudioRecorderSettings();
            //otherFormatSettings.RecordMode
            //otherFormatSettings.fr
            return otherFormatSettings;
            //otherFormatSettings.InputsSettings
        }
    }
}
