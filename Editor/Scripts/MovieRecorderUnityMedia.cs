using System;
using System.Collections;
using System.Collections.Generic;
using FuzzPhyte.Utility;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "OutputFormatMovieEncoderUnityMedia", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/Movie/UnityMedia",order =1)]
    [Serializable]
    public class MovieRecorderUnityMedia : FP_OutputFormatSO
    {
        [Space]
        //mainUnityEncoder
        public UnityEditor.Recorder.Encoder.CoreEncoderSettings CoreEncoderSettings;
        public bool IncludeAudio;
    }
}
