using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "Camera360", menuName = "FuzzPhyte/Recorder/Input/Cam360")]
    [Serializable]
    public class ThreeSixtyRecordData : FP_InputDataSO,IRecorderOutputFormat<Camera360InputSettings>
    {
        public Camera360InputSettings ThreeSixtyCameraSettings;

        public Camera360InputSettings ReturnUnityOutputFormat(GameObject gObject = null)
        {
            return ThreeSixtyCameraSettings;
        }
    }
}
