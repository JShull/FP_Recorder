using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

namespace FuzzPhyte.Recorder.Editor
{
    [CreateAssetMenu(fileName = "CameraOutputData", menuName = "FuzzPhyte/Recorder/OutputFile/CamOutputData")]
    [Serializable]
    public class FP_OutputFileSO : ScriptableObject
    {
        //this would go on an actual editor script
        [HideInInspector]
        public string FileName;
        public List<FPWildCards> WildCardsV;
        public OutputPath OutputPath;
        public virtual void BuildWildCardFileName()
        {
            FileName = "";
            for (int i = 0; i < WildCardsV.Count; i++)
            {
                var curCard = WildCardsV[i];
                string wCard = RecorderPlaceholders.GetWildCardString(curCard);

                if (i == 0)
                {
                    FileName = wCard;
                }
                else
                {
                    FileName += "_" + wCard;
                }
            }
        }
    }
}
