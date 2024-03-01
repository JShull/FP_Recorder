using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "AnimationClip", menuName = "FuzzPhyte/Recorder/Type/AnimationType")]
    [Serializable]
    public class AnimationClipRecorder : FP_OutputFormatSO
    {
        //note: will need the gameobject to be given to us on the editor script side
        //doesn't make sense to add that here
        public bool RecordedComponents;
        public bool ClampedTangents;
        public FPAnimationCompression AnimCompression;
        
    }
}
