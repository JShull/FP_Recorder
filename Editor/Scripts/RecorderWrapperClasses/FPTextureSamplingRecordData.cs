using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Wrapper class for RenderTextureSamplerSettings
    /// </summary>
    [CreateAssetMenu(fileName = "InputTextureSampling", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT2 + "/TextureSampling")]
    [Serializable]
    public class FPTextureSamplingRecordData : FP_InputDataSO
    {
        public RenderTextureSamplerSettings RenderTextureSamplingSettings;
    }
}
