using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{

    [CreateAssetMenu(fileName = "RenderTextureAsset", menuName = "FuzzPhyte/Recorder/Input/RenderTexture")]
    [Serializable]
    public class FPRenderTextureRecordData : FP_InputDataSO
    {
        public RenderTextureInputSettings RenderTextureCameraSettings;
    }
}
