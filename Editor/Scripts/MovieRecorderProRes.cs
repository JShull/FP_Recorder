using System;
using FuzzPhyte.Utility;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "OutputFormatMovieEncoderProRes", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/Movie/ProRes",order =2)]
    [Serializable]
    public class MovieRecorderProRes : FP_OutputFormatSO
    {
        [Space]
        public UnityEditor.Recorder.Encoder.ProResEncoderSettings ProResEncoderSettings;
        public bool IncludeAudio;

        public void Init(bool IncAudio, UnityEditor.Recorder.Encoder.ProResEncoderSettings encoderSettings)
        {
            this.IncludeAudio = IncAudio;
            this.ProResEncoderSettings = encoderSettings;
        }

        public static MovieRecorderProRes CreateInstance(bool incAudio, UnityEditor.Recorder.Encoder.ProResEncoderSettings encSettings)
        {
            var data = ScriptableObject.CreateInstance<MovieRecorderProRes>();
            data.Init(incAudio, encSettings);
            return data;
        }
    }
}
