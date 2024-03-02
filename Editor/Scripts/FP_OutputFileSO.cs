using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEditor;
namespace FuzzPhyte.Recorder.Editor
{
    //[MenuItem("")]
    [CreateAssetMenu(fileName = "CameraOutputData", menuName = FuzzPhyte.Utility.FP_UtilityData.MENU_COMPANY+"/"+FP_RecorderUtility.PRODUCT_NAME+"/"+FP_RecorderUtility.CAT4+"/"+"/CamOutputData")]
    [Serializable]
    public class FP_OutputFileSO : ScriptableObject
    {
        //public CreateAssetMenuAttribute MenuTest;
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
