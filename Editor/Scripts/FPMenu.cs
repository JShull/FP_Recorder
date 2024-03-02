using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FuzzPhyte.Utility;
using FuzzPhyte.Utility.Editor;
using UnityEngine.WSA;

namespace FuzzPhyte.Recorder.Editor
{
    public class FPMenu : MonoBehaviour
    {

        [MenuItem("Window/FuzzPhyte/FP_Recorder/CreateRecorder/Movie", priority = FP_UtilityData.ORDER_SUBMENU_LVL1)]
        static protected void GenerateRecorder()
        {
            // Define the folder path where the asset will be saved
            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);

            // The asset to be created
            var asset = FP_RecorderDataSO.CreateInstance(FPRecorderType.Movie);
            //var asset = ScriptableObject.CreateInstance<FP_RecorderDataSO>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/ARecorder.asset");

            // Create the asset
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            // Focus the asset in the Unity Editor
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            // Optionally, log the creation
            Debug.Log("ExampleAsset created at " + assetPath);
            
        }

        
        [MenuItem("Window/FuzzPhyte/FP_Recorder/CreateConfiguration",priority = FP_UtilityData.ORDER_SUBMENU_LVL1+1)]
        static protected void GenerateRecorderConfiguration()
        {
            // Define the folder path where the asset will be saved
            string folderPath = FP_RecorderUtility.SAMPLESPATH + "/" + FP_RecorderUtility.CAT1;

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            }

            // The asset to be created
            var asset = ScriptableObject.CreateInstance<FP_RecorderSO>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/ARecorderConfiguration.asset");

            // Create the asset
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            // Focus the asset in the Unity Editor
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            // Optionally, log the creation
            Debug.Log("ExampleAsset created at " + assetPath);
        }

        
        
    }
    
}
