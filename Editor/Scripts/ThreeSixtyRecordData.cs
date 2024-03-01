using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "InputCamera360", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT2 + "/Cam360")]
    [Serializable]
    public class ThreeSixtyRecordData : FP_InputDataSO,IRecorderOutputFormat<Camera360InputSettings,GameObject>
    {
        public Camera360InputSettings ThreeSixtyCameraSettings;

        public Camera360InputSettings ReturnUnityOutputFormat(GameObject gObject = null)
        {
            return ThreeSixtyCameraSettings;
        }
    }
}
