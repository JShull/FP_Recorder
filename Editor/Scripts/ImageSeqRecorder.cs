using System;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "ImageSequence", menuName = "FuzzPhyte/Recorder/SettingsType/ImageSeqType")]
    [Serializable]
    public class ImageSeqRecorder : FP_OutputFormatSO
    {
        public FPMediaFileFormat MediaFileFormat;
        //public UnityEditor.Recorder.Input.AudioInput
        public FPCompressionTypes Compression;
        [Range(1,100)]
        public float Quality;
    }
}
