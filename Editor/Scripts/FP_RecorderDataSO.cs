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
        public string RecorderName;
        [Space]
        [Header("Recorder Configuration Files")]
        public FPRecorderType RecorderType;
        [Space]
        [Header("Recorder Data Fields")]
        public FP_InputDataSO InputFormatData;
        public FP_OutputFormatSO OutputFormatData;
        public FP_OutputFileSO OutputCameraData;

        [HideInInspector]
        public int NumberOfCameras = 1;
        [HideInInspector]
        [Tooltip("This is a reference to the GameObject that we might need for an animation clip")]
        public GameObject AnimationClipGameObject;
        public void Init(FPRecorderType recorderType,string name)
        {
            this.RecorderType = recorderType;
            this.RecorderName = name;
        }

        public static FP_RecorderDataSO CreateInstance(FPRecorderType recType, string recordName)
        {
            var data = ScriptableObject.CreateInstance<FP_RecorderDataSO>();
            data.Init(recType,recordName);
            return data;
        }
        public void RemoveReferencesAndDelete()
        {

        }

    }
}
