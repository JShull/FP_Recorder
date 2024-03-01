using System;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "ProResEncoder", menuName = "FuzzPhyte/Recorder/Type/Encoder/ProRes")]
    [Serializable]
    public class MovieRecorderProRes : FP_OutputFormatSO
    {
        [Space]
        public UnityEditor.Recorder.Encoder.ProResEncoderSettings ProResEncoderSettings;
        public bool IncludeAudio;
    }
}
