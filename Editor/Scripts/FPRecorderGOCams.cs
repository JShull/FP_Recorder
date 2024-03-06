using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace FuzzPhyte.Recorder.Editor
{
    [Serializable]
    public class FPRecorderGOCams 
    {
        public List<string> GUIGameObjects;

        public FPRecorderGOCams()
        {
            GUIGameObjects = new List<string>();
        }
    }
}
