using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEditor.Recorder.Input;
namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Wrapper class for AnimationClip
    /// </summary>
    [CreateAssetMenu(fileName = "OutputFormatAnimationClip", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/AnimationClip")]
    [Serializable]
    public class AnimationClipRecorder : FP_InputDataSO, IRecorderOutputFormat<AnimationRecorderSettings,GameObject>
    {
        public bool RecordedHierarchy=true;
        public bool ClampedTangents=true;
        public AnimationInputSettings.CurveSimplificationOptions AnimCompresion;
        public AnimationRecorderSettings ReturnUnityOutputFormat(GameObject animData)
        {
            var newAnimRecordSettings = new AnimationRecorderSettings();
            var newInputSettings = new AnimationInputSettings();
            //pass settings
            newInputSettings.SimplyCurves = AnimCompresion;
            newInputSettings.ClampedTangents = ClampedTangents;
            newInputSettings.Recursive = RecordedHierarchy;
            if (animData != null)
            {
                newInputSettings.gameObject = animData;
            }
            
            newAnimRecordSettings.AnimationInputSettings = newInputSettings;
            return newAnimRecordSettings;
        }

        public void Init(bool recordedHierarchy, bool clampedTangents,AnimationInputSettings.CurveSimplificationOptions cCompression,GameObject gObject=null)
        {
            this.RecordedHierarchy = recordedHierarchy;
            this.ClampedTangents = clampedTangents;
            this.AnimCompresion = cCompression;
            ReturnUnityOutputFormat(gObject);
        }

        public static AnimationClipRecorder CreateInstance(bool recordHierarchy,bool clampedTangents, AnimationInputSettings.CurveSimplificationOptions curveCompression,GameObject gaObject=null)
        {
            var data = ScriptableObject.CreateInstance<AnimationClipRecorder>();
            data.Init(recordHierarchy,clampedTangents,curveCompression,gaObject);
            return data;
        }
    }
}
