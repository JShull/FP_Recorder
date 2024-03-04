using System.Collections.Generic;
using UnityEngine;
using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder;

namespace FuzzPhyte.Recorder.Editor
{
    [Serializable]
    public abstract class FP_InputDataSO : ScriptableObject
    {
        [Space]
        [Header("INPUT")]
        public FPInputSettings Source;
    }
}

