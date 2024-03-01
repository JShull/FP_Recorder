using System;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "ImageSequence", menuName = "FuzzPhyte/Recorder/Type/ImageSeqType")]
    [Serializable]
    public class ImageSeqRecorder : FP_OutputFormatSO
    {
        public UnityEditor.Recorder.ImageRecorderSettings ImageRecorderSettings;
        //public UnityEditor.Recorder.Input.AudioInput

    }
}
