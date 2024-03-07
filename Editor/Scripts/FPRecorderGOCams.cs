using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Hold references to camera/gameobjects in scene
    /// </summary>
    [Serializable]
    public class FPRecorderGOCams 
    {
        public List<string> GameObjectNames;

        public FPRecorderGOCams()
        {
            GameObjectNames = new List<string>();
        }
    }
}
