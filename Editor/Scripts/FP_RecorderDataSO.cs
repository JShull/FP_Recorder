using UnityEngine;
using System;
using FuzzPhyte.Utility;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "SingleRecorderSetup", menuName = "FuzzPhyte/Recorder/Configuration/ARecorder",order =10)]
    [Serializable]
    public class FP_RecorderDataSO : ScriptableObject
    {
        public FP_Camera CameraData;
        
 
        //public UnityEditor.Recorder.
        [Space]
        [Header("Recorder Configuration Files")]
        public FPRecorderType RecorderType;
        [Space]
        [Header("Recorder Data Fields")]
        public FP_InputDataSO InputFormatData;
        public FP_OutputFormatSO OutputFormatData;
        public FP_OutputFileSO OutputCameraData;

        public void SetupRecorderData()
        {
            //RecorderDetails.mod
        }
    }
}
