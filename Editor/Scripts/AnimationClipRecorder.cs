using System;
using UnityEditor.Recorder;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "AnimationClip", menuName = "FuzzPhyte/Recorder/SettingsType/AnimationType")]
    [Serializable]
    public class AnimationClipRecorder : FP_OutputFormatSO, IRecorderOutputFormat<AnimationRecorderSettings>
    {
        //note: will need the gameobject to be given to us on the editor script side
        //doesn't make sense to add that here
        public bool RecordedHierarchy=true;
        public bool ClampedTangents=true;
        //public FPAnimationCompression AnimCompression;
        public UnityEditor.Recorder.Input.AnimationInputSettings.CurveSimplificationOptions AnimCompresion;
        //public UnityEditor.Recorder.Input.AnimationInputSettings AnimSettings;
        public AnimationRecorderSettings ReturnUnityOutputFormat(GameObject animData=null)
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
