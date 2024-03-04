using UnityEngine;
using System;
using FuzzPhyte.Utility;
namespace FuzzPhyte.Recorder.Editor
{
    
    [CreateAssetMenuAttribute(menuName = FP_UtilityData.MENU_COMPANY + "/ARecorder",order =10)]
    //[UnityEditor.MenuItem("ScriptableObjects/Category/Special/MyObject")]
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

        public void Init(FPRecorderType recorderType)
        {
            this.RecorderType = recorderType;    
        }

        public static FP_RecorderDataSO CreateInstance(FPRecorderType recType)
        {
            var data = ScriptableObject.CreateInstance<FP_RecorderDataSO>();
            data.Init(recType);
            return data;
        }
        public void RemoveReferencesAndDelete()
        {

        }

    }
}
