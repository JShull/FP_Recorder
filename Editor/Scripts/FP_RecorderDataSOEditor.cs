using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CustomEditor(typeof(FP_RecorderDataSO))]
    public class FP_RecorderDataSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Draws the default inspector

            FP_RecorderDataSO scriptableObject = (FP_RecorderDataSO)target;

            if (GUILayout.Button("Delete Data Files and Reset"))
            {
                RemoveAndDeleteAssets(scriptableObject);
            }
        }

        private static void RemoveAndDeleteAssets(FP_RecorderDataSO dataSO)
        {

            //try to delete myself
            var selected = Selection.activeObject as FP_RecorderDataSO;
            if (selected != null)
            {
                
                // Deleting referenced assets
                DeleteAsset(dataSO.InputFormatData);
                DeleteAsset(dataSO.OutputFormatData);
                DeleteAsset(dataSO.OutputCameraData);

                // Optional: Remove the references in the ScriptableObject after deletion
                dataSO.InputFormatData = null;
                dataSO.OutputFormatData = null;
                dataSO.OutputCameraData = null;
                // Save changes to the ScriptableObject
                EditorUtility.SetDirty(dataSO);
                AssetDatabase.SaveAssets();
            }
           
        }

        private static void DeleteAsset(UnityEngine.Object asset)
        {
            if (asset != null)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }
    }
}
