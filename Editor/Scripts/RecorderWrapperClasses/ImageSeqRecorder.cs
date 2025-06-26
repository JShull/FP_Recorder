using System;
using FuzzPhyte.Utility;
using UnityEngine;
using UnityEditor.Recorder;
namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Wrapper class for ImageRecorderSettings
    /// </summary>
    [CreateAssetMenu(fileName = "OutputFormatImageSequence", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/ImageSeqType")]
    [Serializable]
    public class ImageSeqRecorder : FP_OutputFormatSO
    {
        public ImageRecorderSettings.ImageRecorderOutputFormat MediaFileFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
        public CompressionUtility.EXRCompressionType Compression = CompressionUtility.EXRCompressionType.Zip;
        [Range(1,100)]
        public int Quality;

        public void Init(ImageRecorderSettings.ImageRecorderOutputFormat formatMedia,CompressionUtility.EXRCompressionType compression, int qty)
        {
            this.MediaFileFormat = formatMedia;
            this.Compression = compression;
            this.Quality = qty;
            this.EncoderType = FPEncoderType.ImageEncoder;
        }

        public static ImageSeqRecorder CreateInstance(ImageRecorderSettings.ImageRecorderOutputFormat format, CompressionUtility.EXRCompressionType comp, int quant)
        {
            var data = ScriptableObject.CreateInstance<ImageSeqRecorder>();
            data.Init(format,comp,quant);
            return data;
        }
    }
}
