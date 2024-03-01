using System;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    //Base Class
    [Serializable]
    public abstract class FP_OutputFormatSO : ScriptableObject
    {
        
        public bool EnabledRecorder = true;
    }
}
