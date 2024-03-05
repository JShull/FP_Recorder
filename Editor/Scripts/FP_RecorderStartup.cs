using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [InitializeOnLoad]
    static class FP_RecorderStartup
    {
        static FP_RecorderStartup()
        {
            /*
            if (!SessionState.GetBool(FP_RecorderUtility.PRODUCT_NAME + "_STARTUP", false))
            {
                Debug.Log($"First Init. on FPRecorderStartup");
                FPMenu.SetupSettingsFileOnBoot();
                SessionState.SetBool(FP_RecorderUtility.PRODUCT_NAME + "_STARTUP", true);
            }
            */
        }
    }
}
