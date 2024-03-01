using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "GifEncoder", menuName = "FuzzPhyte/Recorder/Type/Encoder/Gif")]
    [Serializable]
    public class MovieRecorderGif : FP_OutputFormatSO
    {
        [Space]
        public UnityEditor.Recorder.Encoder.GifEncoderSettings GifEncoderSettings;
    }

}
