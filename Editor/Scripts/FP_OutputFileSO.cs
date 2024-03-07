using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEditor;
using Microsoft.SqlServer.Server;
using System.IO;

namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Wrapper class that holds relative data associated with 'OutputPath' for the UnityEditor.Recorder
    /// </summary>
    [CreateAssetMenu(fileName = "CameraOutputData", menuName = FuzzPhyte.Utility.FP_UtilityData.MENU_COMPANY+"/"+FP_RecorderUtility.PRODUCT_NAME+"/"+FP_RecorderUtility.CAT4+"/"+"/CamOutputData")]
    [Serializable]
    public class FP_OutputFileSO : ScriptableObject
    {
        //Holds value for path/file associated with the OutputPath
        public string FileName;
        public List<FPWildCards> WildCardsV;
        public OutputPath OutputPath;
        public virtual void BuildWildCardFileName()
        {
            FileName = "";
            for (int i = 0; i < WildCardsV.Count; i++)
            {
                var curCard = WildCardsV[i];
                string wCard = FP_RecorderUtility.GetWildCardString(curCard);

                if (i == 0)
                {
                    FileName = wCard;
                }
                else
                {
                    FileName += "_" + wCard;
                }
            }
            var mediaOutputFolder = Path.Combine(Application.dataPath,"Recordings/"+FileName);
            FileName = mediaOutputFolder;
        }
        public void Init(List<FPWildCards>wCards, OutputPath.Root anOutput)
        {
            this.WildCardsV = new List<FPWildCards>();
            this.OutputPath = new OutputPath
            {
                
            };
           
            for(int i = 0; i < wCards.Count; i++)
            {
                this.WildCardsV.Add(wCards[i]);
            }
            //now actually build it
            BuildWildCardFileName();

        }

        public static FP_OutputFileSO CreateInstance(List<FPWildCards> wildCards, OutputPath.Root anOutputPath)
        {
            var data = ScriptableObject.CreateInstance<FP_OutputFileSO>();
            data.Init(wildCards,anOutputPath);
            return data;
        }



    }
}
