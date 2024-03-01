using System;
using UnityEngine;
using UnityEditor.Recorder;
using FuzzPhyte.Utility;
using UnityEditor;
namespace FuzzPhyte.Recorder.Editor
{

    [CreateAssetMenu(fileName = "OutputFormatAudio", menuName = FP_UtilityData.MENU_COMPANY+ "/"+FP_RecorderUtility.PRODUCT_NAME+ "/"+FP_RecorderUtility.CAT3+"/Audio")]
    [Serializable]
    public class AudioRecorder : FP_OutputFormatSO, IRecorderOutputFormat<AudioRecorderSettings,GameObject>
    {
        [Space]
        //public AudioRecorderSettings AudioRecorderSettings;
        public FPAudioFormat Format;

        //public string myNameIS = FP_RecorderUtility.PRODUCT_NAME;
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
