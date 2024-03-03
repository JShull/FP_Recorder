using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    //Base Class
    [Serializable]
    public abstract class FP_OutputFormatSO : ScriptableObject
    {
        
        public bool EnabledRecorder = true;
        //public abstract void Init(bool recordedHierarchy, bool clampedTangents, AnimationInputSettings.CurveSimplificationOptions cCompression, GameObject gObject = null);

    }
}
