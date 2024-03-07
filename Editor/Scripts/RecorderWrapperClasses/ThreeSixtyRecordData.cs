using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    //wrapper class to hold data for Camera360InputSettings
    [CreateAssetMenu(fileName = "InputCamera360", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT2 + "/Cam360")]
    [Serializable]
    public class ThreeSixtyRecordData : FP_InputDataSO,IRecorderOutputFormat<Camera360InputSettings,GameObject>
    {
        public Camera360InputSettings ThreeSixtyCameraSettings;

        public Camera360InputSettings ReturnUnityOutputFormat(GameObject gObject = null)
        {
            return ThreeSixtyCameraSettings;
        }
        public void Init(Camera360InputSettings camSettings,GameObject gaObject=null)
        {
            this.ThreeSixtyCameraSettings = camSettings;
            this.Source = FPInputSettings.a360View;
        }

        public static ThreeSixtyRecordData CreateInstance(Camera360InputSettings cameraSettings)
        {
            var data = ScriptableObject.CreateInstance<ThreeSixtyRecordData>();
            data.Init(cameraSettings);
            return data;
        }
    }
}
