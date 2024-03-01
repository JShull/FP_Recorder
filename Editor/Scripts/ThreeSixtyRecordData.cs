using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "Camera360", menuName = "FuzzPhyte/Recorder/Input/Cam360")]
    [Serializable]
    public class ThreeSixtyRecordData : FP_InputDataSO
    {
        public Camera360InputSettings ThreeSixtyCameraSettings;
    }
}
