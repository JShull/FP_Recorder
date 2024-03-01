using System;
using FuzzPhyte.Utility;
using UnityEngine;
using UnityEngine.Recorder;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "AllRecorderSettings", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT1 + "/RecorderSettings",order =11)]
    [Serializable]
    public class FP_RecorderSO : ScriptableObject
    {
        [Space]
        [Header("General Recorder Settings")]
        public UnityEditor.Recorder.RecordMode RecordMode;
        [Header("FrameRate")]
        public UnityEditor.Recorder.FrameRatePlayback Playback;
        public UnityEditor.Recorder.RecorderSettings GeneralRecorderSettings;
        public bool CapFPS = true;
    }
}
