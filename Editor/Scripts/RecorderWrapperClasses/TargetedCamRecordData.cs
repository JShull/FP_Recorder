using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// wrapper class for data related to CameraInputSettings
    /// </summary>
    [CreateAssetMenu(fileName = "InputTargetedCamera", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT2 + "/TargetedCamera")]
    [Serializable]
    public class TargetedCamRecordData : FP_InputDataSO
    {
        public CameraInputSettings TargetedCameraSettings;
    }
}
