using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "TextureSampling", menuName = "FuzzPhyte/Recorder/Input/TextureSampling")]
    [Serializable]
    public class FPTextureSamplingRecordData : FP_InputDataSO
    {
        public RenderTextureSamplerSettings RenderTextureSamplingSettings;
    }
}
