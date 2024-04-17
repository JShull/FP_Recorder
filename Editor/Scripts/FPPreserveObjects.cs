using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using FuzzPhyte.Utility.Editor;
using System.IO;

namespace FuzzPhyte.Recorder.Editor
{
    [InitializeOnLoad]
    public class FPPreserveObjects
    {
        /// <summary>
        /// generic constructor for Instance
        /// </summary>
        private FPPreserveObjects() { }
        
        private static FPPreserveObjects _instance;
        private static bool isSubscribed = false;
        private static Dictionary<string, GameObject> preservedObjects = new Dictionary<string, GameObject>();
        private static Dictionary<string,Vector3> preservedPositions = new Dictionary<string,Vector3>();
        private static Dictionary<string,string> preservedTags = new Dictionary<string, string>();
        private static Dictionary<string,int> preservedTagsIndex = new Dictionary<string, int>();
        private static bool saveJSONOnFinish = false;
        //private static Transform parentItem;
        private static int currentCams=0;
        private static int startingCamCount =0;
        private static string userFileName = "Default";
        public static FPPreserveObjects Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FPPreserveObjects();
                    SubscribeEvents();
                }
                return _instance;
            }
        }
        
#region Subscription Events
        private static void SubscribeEvents()
        {
            if (!isSubscribed)
            {
                EditorApplication.playModeStateChanged += Instance.OnPlayModeStateChanged;
                isSubscribed = true;
            }
        }
        public static void UnsubscribeEvents()
        {
            if (isSubscribed)
            {
                EditorApplication.playModeStateChanged -= Instance.OnPlayModeStateChanged;
                isSubscribed = false;
            }
        }
#endregion
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                //preservedObjects.Clear();
                PreserveObjects(FP_RecorderUtility.BaseCamName,FP_RecorderUtility.CamTAG,currentCams);
                preservedObjects.Clear();
                preservedPositions.Clear();
                currentCams = 0;
                isSubscribed = false;
                preservedTags.Clear();
                preservedTagsIndex.Clear();
                saveJSONOnFinish=false;
                startingCamCount=0;
                userFileName = "Default";
            }
            
        }
        /// <summary>
        /// Make a copy of everything and instantiate it after we end 
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="baseTag"></param>
        /// <param name="currentCamsInScene"></param>
        /// <param name="camIndex"></param>
        private void PreserveObjects(string baseName,string baseTag,int currentCamsInScene, int camIndex=-1)
        {
            //deal with tags
            
            List<GameObject> allValues = preservedObjects.Values.ToList();
            List<Vector3> allPositions = preservedPositions.Values.ToList();
            List<string> allTags = preservedTags.Values.ToList();
            List<int> allTagIndices = preservedTagsIndex.Values.ToList();
            
            //5, "FPCamera", FP_RecorderUtility.CamTAG,settingsData.RendererIndex
            Debug.Log($"Preserving {allValues.Count} objects");
            if(allValues.Count==0)
            {
                Debug.Log($"No Objects to Preserve");
                return;
            }
            //spawn here not on the other item
            //FPMenu.GenerateGameObjectRuntime(parentItem,allValues,"FPCamera",FP_RecorderUtility.CamTAG);

            //spawn
            //local path
            var fakeCamPath = Path.Combine("Prefab","FakeCam");
            var fakeParentPath = Path.Combine("Prefab","FakeParent");
            Debug.Log($"Parent Path: {fakeParentPath}, child cam Path: {fakeCamPath}");
            var parentFake = Resources.Load<GameObject>("Prefab/FakeParent");
            var camFake = Resources.Load<GameObject>("Prefab/FakeCam");
            GameObject newParent = GameObject.Instantiate(parentFake);
            newParent.name = baseName + "_" + allValues.Count + "_cameraParent";
            newParent.transform.position = new Vector3(0, 0, 0);
            //make sure our cameraData is good to go

            FPMenu.GenerateGameObjectRunTime();
            Debug.Log($"Loop AllValues Count= {allValues.Count} and I have new tags to add of {allTags.Count}");
            for(int i=0;i<allTags.Count;i++)
            {
                FPGenerateTag.CreateTag(allTags[i]);
            }
            for(int i = 0; i < allValues.Count; i++)
            {
                //var passedGO = allValues[i];
                //spawn a prefab from resources 
                //GameObject newObj = PrefabUtility.InstantiatePrefab(camFake) as GameObject;
                GameObject newObj = GameObject.Instantiate(camFake);;


                //GameObject newObj = GameObject.Instantiate(allValues[i]) as GameObject;
                int camTagNumber = currentCamsInScene + i;
                newObj.name = baseName +"_"+i.ToString()+ "_" + camTagNumber.ToString(); // Unique name using GUID
                FPMenu.AddGameObjectCameraData(newObj.name);
                //cameraData.GameObjectNames.Add(newObj.name);
                
                newObj.tag = baseTag + camTagNumber.ToString(); // Assign the tag
                Debug.Log($"Tag assigned: {newObj.tag}");
                newObj.transform.SetParent(newParent.transform);
                newObj.transform.localPosition = allPositions[i];
                //turn on the camera and turn off a visual renderer
                if(newObj.GetComponent<Camera>())
                {
                    newObj.GetComponent<Camera>().enabled = true;
                }
                if(newObj.GetComponent<MeshRenderer>())
                {
                    newObj.GetComponent<MeshRenderer>().enabled = false;
                }
                if(newObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>())
                {
                    newObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
#if UNITY_PIPELINE_URP
                var camData = newObj.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                if (camData != null)
                {
                    camData.SetRenderer(camIndex);
                }
#endif
            }

            //need to make sure we don't replace the recorder data and add to it instead
            //force settings update
            var curRecorderData = FPMenu.GetCurrentRecorderSettings();
            Debug.Log($"Current recorder setting Counts: {curRecorderData.RecorderSettings.ToList().Count}");
            FPMenu.ForceSettingsUpdate(curRecorderData);
           
            //add camera to recorder
            for(int i=0;i<allTagIndices.Count;i++)
            {
                FPMenu.RunTimeAddSingleCamera(allTagIndices[i],allTags[i]);
            }
             //save JSON?
             
            if(saveJSONOnFinish){
                //grab current data
                FPMenu.RuntimeWriteRecorderAsset(userFileName);
            }
            
            //spawn end
            
        }
        /// <summary>
        /// At runtime we can call this method to add the item from our GUI referencing the instance  
        /// </summary>
        /// <param name="obj"></param> <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void AddObjectForTracking(GameObject obj)
        {
            //confirm that the object is not already being tracked we are using the tag from the object as the Key
            if (!preservedObjects.ContainsKey(obj.tag))
            {
                preservedObjects.Add(obj.tag, obj);
                preservedPositions.Add(obj.tag,obj.transform.position);
            }
        }
        public void AddTagIndex(string tag, int index)
        {
            if(!preservedTagsIndex.ContainsKey(tag))
            {
                preservedTagsIndex.Add(tag,index);
            }
        }

        public void UpdateStartingCamCount(int passedCam)
        {
            startingCamCount = passedCam;
        }
        public void UpdateCamerasInScene(int numberCamsInScene)
        {
            currentCams = numberCamsInScene;
        }
        public void AddTag(string tag)
        {
            if(!preservedTags.ContainsKey(tag)){
                preservedTags.Add(tag,tag);
            } 
        }
        public void UpdateUserFileName(string passedFileName)
        {
            userFileName = passedFileName;
        }
        public void SaveJSONUpdate(bool saveJSON)
        {
            saveJSONOnFinish = saveJSON;
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        ~FPPreserveObjects()
        {
            UnsubscribeEvents();
        }
    }
}
