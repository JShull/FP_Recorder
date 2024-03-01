using System;
using FuzzPhyte.Utility;
using UnityEditor.Recorder;
using UnityEngine;

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
        public UnityEditor.Recorder.Input.AnimationInputSettings.CurveSimplificationOptions AnimCompresion;
        //public UnityEditor.Recorder.Input.AnimationInputSettings AnimSettings;
        public AnimationRecorderSettings ReturnUnityOutputFormat(GameObject animData)
        {
            var newAnimRecordSettings = new AnimationRecorderSettings();
            var newInputSettings = new UnityEditor.Recorder.Input.AnimationInputSettings();
            //pass settings
            newInputSettings.SimplyCurves = AnimCompresion;
            newInputSettings.ClampedTangents = ClampedTangents;
            newInputSettings.Recursive = RecordedHierarchy;
            newInputSettings.gameObject = animData;
            newAnimRecordSettings.AnimationInputSettings = newInputSettings;
            return newAnimRecordSettings;
        }
    }
}
