using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "CameraTargetedCamera", menuName = "FuzzPhyte/Recorder/Input/TargetedCamera")]
    [Serializable]
    public class TargetedCamRecordData : FP_InputDataSO
    {
        public CameraInputSettings TargetedCameraSettings;
    }
}
