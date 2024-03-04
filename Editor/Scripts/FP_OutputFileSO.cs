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
            var mediaOutputFolder = Path.Combine(Application.dataPath,"Recordings/"+FileName);
            FileName = mediaOutputFolder;
            //OutputPath.Equals(OutputPath.Root.AssetsFolder);
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
