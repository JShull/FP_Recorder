using System;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Base class for wrappers associated with the UnityEditor.Recorder Package
    /// </summary>
    [Serializable]
    public abstract class FP_OutputFormatSO : ScriptableObject
    {
        public bool EnabledRecorder = true;
        public FPEncoderType EncoderType;
    }
}
