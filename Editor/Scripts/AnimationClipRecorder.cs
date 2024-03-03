using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEditor.Recorder.Input;
namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "OutputFormatAnimationClip", menuName = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/" + FP_RecorderUtility.CAT3 + "/AnimationClip")]
    [Serializable]
    public class AnimationClipRecorder : FP_OutputFormatSO, IRecorderOutputFormat<AnimationRecorderSettings,GameObject>
    {
        //note: will need the gameobject to be given to us on the editor script side
        //doesn't make sense to add that here
        public bool RecordedHierarchy=true;
        public bool ClampedTangents=true;
        //public FPAnimationCompression AnimCompression;
        public AnimationInputSettings.CurveSimplificationOptions AnimCompresion;
        //public UnityEditor.Recorder.Input.AnimationInputSettings AnimSettings;
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
