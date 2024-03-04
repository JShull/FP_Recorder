using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FuzzPhyte.Utility.Editor;
namespace FuzzPhyte.Recorder.Editor
{
    [CustomEditor(typeof(FP_RecorderDataSO))]
    public class FP_RecorderDataSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Draws the default inspector
            GUIStyle deleteFilesButtonStyle = new GUIStyle(GUI.skin.button);
            deleteFilesButtonStyle.normal.textColor = Color.red; // Text color for the normal state
            deleteFilesButtonStyle.hover.textColor = Color.yellow; // Text color when hovered
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.margin = new RectOffset(15, 5, 5, 5);
            boxStyle.normal.textColor = Color.white;
            boxStyle.hover.textColor = Color.blue;
            boxStyle.border = new RectOffset(20, 7, 7, 7);
            boxStyle.focused.textColor = Color.blue;

            EditorGUILayout.Space(20);
            FP_Utility_Editor.DrawUILine(Color.red, 5, 5);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(boxStyle);

            FP_RecorderDataSO scriptableObject = (FP_RecorderDataSO)target;
            scriptableObject.NumberOfCameras = EditorGUILayout.IntField("Number of Cameras?: ", scriptableObject.NumberOfCameras);
            if(scriptableObject.RecorderType == FPRecorderType.AnimationClip)
            {
                EditorGUILayout.Space(10);
                scriptableObject.AnimationClipGameObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", scriptableObject.AnimationClipGameObject, typeof(GameObject), true);
                EditorGUILayout.Space(10);    
            }
            
            if (GUILayout.Button($"Add {scriptableObject.NumberOfCameras} Cameras to the Recorder?"))
            {

            }
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("**CAUTION**", deleteFilesButtonStyle);
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Delete Data Files and Reset",deleteFilesButtonStyle))
            {
                RemoveAndDeleteAssets(scriptableObject);
            }
            EditorGUILayout.EndVertical();

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
