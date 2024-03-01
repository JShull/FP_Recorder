using System;
using System.Collections;
using System.Collections.Generic;
using FuzzPhyte.Utility;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "OutputFormatMovieEncoderGif", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/Movie/Gif",order =3)]
    [Serializable]
    public class MovieRecorderGif : FP_OutputFormatSO
    {

        [Space]
        public UnityEditor.Recorder.Encoder.GifEncoderSettings GifEncoderSettings;
    }

}
