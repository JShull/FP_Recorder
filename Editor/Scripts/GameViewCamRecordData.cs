using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "CameraGameView", menuName = "FuzzPhyte/Recorder/Input/GameView")]
    [Serializable]
    public class GameViewCamRecordData : FP_InputDataSO
    {
        public GameViewInputSettings GameViewSettings;
    }
}
