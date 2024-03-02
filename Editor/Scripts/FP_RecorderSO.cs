using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.Recorder;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "AllRecorderSettings", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT1 + "/RecorderSettings",order =11)]
    [Serializable]
    public class FP_RecorderSO : ScriptableObject,IRecorderOutputFormat<RecorderControllerSettings,GameObject>
    {
        [Space]
        
        [Header("FrameRate")]
        public FrameRatePlayback Playback;
        [Header("General Recorder Settings")]
        public RecordMode RecordMode;
        [HideInInspector]
        public RecorderControllerSettings MyPreset;
        [Space]
        public int TargetFrame;
        [Space]
        [Header("FPS Intervals")]
        public int StartIntervalFrame;
        public int EndIntervalFrame;
        [Space]
        [Header("Time Intervals")]
        public float StartTimeInterval;
        public float EndTimeIntervarl;
        [Space]
        public bool CapFPS = true;

        public RecorderControllerSettings ReturnUnityOutputFormat(GameObject inputParameters)
        {
            MyPreset = new RecorderControllerSettings();
            switch (RecordMode)
            {
                case RecordMode.Manual:
                    MyPreset.SetRecordModeToManual();
                    break;
                case RecordMode.SingleFrame:
                    MyPreset.SetRecordModeToSingleFrame(TargetFrame);
                    break;
                case RecordMode.FrameInterval:
                    MyPreset.SetRecordModeToFrameInterval(StartIntervalFrame, EndIntervalFrame);
                    break;
                case RecordMode.TimeInterval:
                    MyPreset.SetRecordModeToTimeInterval(StartTimeInterval, EndTimeIntervarl);
                    break;
            }
            MyPreset.FrameRatePlayback = Playback;
           
            return MyPreset;

        }
    }
}
