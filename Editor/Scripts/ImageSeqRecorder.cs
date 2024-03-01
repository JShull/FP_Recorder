using System;
using FuzzPhyte.Utility;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "OutputFormatImageSequence", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/ImageSeqType")]
    [Serializable]
    public class ImageSeqRecorder : FP_OutputFormatSO
    {
        public FPMediaFileFormat MediaFileFormat = FPMediaFileFormat.PNG;
        public FPCompressionTypes Compression;
        [Range(1,100)]
        public float Quality;
    }
}
