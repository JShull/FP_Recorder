using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Wrapper class for GameViewInputSettings
    /// </summary>
    [CreateAssetMenu(fileName = "InputGameView", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT2 + "/GameView")]
    [Serializable]
    public class GameViewCamRecordData : FP_InputDataSO
    {
        public GameViewInputSettings GameViewSettings;
    }
}
