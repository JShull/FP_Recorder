using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{

    [CreateAssetMenu(fileName = "InputRenderTextureAsset", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT2 + "/RenderTexture")]
    [Serializable]
    public class FPRenderTextureRecordData : FP_InputDataSO, IRecorderOutputFormat<RenderTextureInputSettings,RenderTexture>
    {
        public RenderTextureInputSettings RenderTextureCameraSettings;

        public RenderTextureInputSettings ReturnUnityOutputFormat(RenderTexture passedRendererTex)
        {
            RenderTextureCameraSettings.RenderTexture = passedRendererTex;
            return RenderTextureCameraSettings;
        }
    }
}
