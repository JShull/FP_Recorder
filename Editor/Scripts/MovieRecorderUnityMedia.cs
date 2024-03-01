using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "UnityMedia", menuName = "FuzzPhyte/Recorder/Type/Encoder/UnityMedia")]
    [Serializable]
    public class MovieRecorderUnityMedia : FP_OutputFormatSO
    {
        [Space]
        //mainUnityEncoder
        public UnityEditor.Recorder.Encoder.CoreEncoderSettings CoreEncoderSettings;
        public bool IncludeAudio;
    }
}
