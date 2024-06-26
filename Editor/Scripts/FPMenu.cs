using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder.Input;
using FuzzPhyte.Utility;
using FuzzPhyte.Utility.Editor;
using UnityEditor.Recorder;
using System.IO;
using System;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;
namespace FuzzPhyte.Recorder.Editor
{
    /// <summary>
    /// Main class that assigns and aligns data between UnityEditor.Recorder and UnityEditor.Prefs and our JSON backup
    /// </summary>
    public class FPMenu : MonoBehaviour, IFPProductEditorUtility
    {
        //LOCAL CACHED SERIALIZABLE DATA FILES
        private static RecorderControllerSettings settings;
        private static FPRecorderSettingsJSON settingsData;
        private static FPRecorderGOCams cameraData;
        #region Editor Menu Configuration
        //EDITOR PREFERENCES
        private const string SettingName = "FP_RecorderSettings";
        private const string NumberCamTagsFileName = "FP_CamTagsCount";
        private const string NumberCamerasSceneFileName = "FP_NumCamScene";
        private const string JSONEditorName = "FP_RecorderSettingsJSON";
        private const string HaveSettingsFileName = "FP_HaveSettings";
        private const string CameraSettingsFileName = "FP_CamSettings";
        //
        private const string MenuB = FP_UtilityData.MENU_COMPANY+"/"+FP_RecorderUtility.PRODUCT_NAME;
        private const string SetupMenuB = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/Setup";
        private const string SetupAddTenCameras = SetupMenuB + "/"+FP_RecorderUtility.SUB2+"/Plus 10 CameraTags";
        private const string SetupAddFiveCameras = SetupMenuB + "/" + FP_RecorderUtility.SUB2 +"/Plus 5 CameraTags";
        private const string SetupRemoveTenCameras = SetupMenuB + "/" + FP_RecorderUtility.SUB2 + "/Remove 10 CameraTags";
        private const string SetupRemoveFiveCameras = SetupMenuB + "/" + FP_RecorderUtility.SUB2 + "/Remove 5 CameraTags";
        //
        private const string LoadRecorderFromData = SetupMenuB + "/Data/Load from Editor";
        private const string WriteBackupJSON = SetupMenuB + "/Data/Save JSON Backup";
        private const string ReadBackupJSON = SetupMenuB + "/Data/Read JSON Backup";
        //reset
        private const string RemoveAllFPCameras = SetupMenuB + "/Reset/Reset All FPCameraTags";
        private const string ResetAllEditorData = SetupMenuB + "/Reset/Reset All Editor DATA";
        private const string ResetEVERYTHING = SetupMenuB + "/Reset/Reset Everything";
        //fake menus
        private const string DateOutputHeader = MenuB + "/DATA MENU";
        private const string CreateOutputHeader = MenuB + "/CREATE MENU";
        //
        private const string OutputFormat = MenuB + "/"+FP_RecorderUtility.SUB0 +" "+ FP_RecorderUtility.CAT3;
        private const string InputFile = MenuB + "/" + FP_RecorderUtility.SUB0 + " " + FP_RecorderUtility.CAT2;
        private const string RecorderType = MenuB + "/" + FP_RecorderUtility.SUB0 + " " + FP_RecorderUtility.CAT1;
        private const string OutputFile = MenuB + "/" + FP_RecorderUtility.SUB0 + " " + FP_RecorderUtility.CAT4;
        private const string UnityRecorder = MenuB + "/" + FP_RecorderUtility.SUB0 + " " + FP_RecorderUtility.CAT5;
        //Configuration
        private const string DefaultConfigurationFileMenu = MenuB + "/"+ FP_RecorderUtility.SUB0 + " " + FP_RecorderUtility.CAT0;
        private const string CGenericConfigMenu = DefaultConfigurationFileMenu + "/Default Config.";
        private const string C360ConfigMenu = DefaultConfigurationFileMenu + "/360 Config.";
        //Add Recorder
        private const string AddRecorderFileMenu = MenuB + "/Create/ " + FP_RecorderUtility.CAT5;
        private const string AddAnySingleRecorderToRecorderSettings = AddRecorderFileMenu + "/Add Selected Recorder";
        private const string CreateAndAdd360RecorderToRecorderSettings = UnityRecorder + "/Add Unity Recorder: 360";
        //additional gameobject cameras
        private const string AddGameObjectFileMenu = MenuB + "/Create and Place/"+FP_RecorderUtility.CAT6;
        private const string FiveCameraSpawnSettings = AddGameObjectFileMenu + "/5 x 360-Cameras";
        private const string TenCameraSpawnSettings = AddGameObjectFileMenu + "/10 x 360-Cameras";
        //Input Format
        private const string CamThreeSixtyInput = InputFile + "/360Cam";
        private const string RecordTakeOutputFile = OutputFile + "/<Recorder><Take> File";

        //Output Format
        private const string MovieOutputMenu = OutputFormat + "/MovieEncoderUnity";
        private const string MovieOutputProResMenu = OutputFormat + "/MovieEncoderProRes";
        private const string MovieOutputGif = OutputFormat + "/MovieEncoderGif";
        private const string ImageSeqOutput = OutputFormat + "/ImageSequence";
        private const string AudioOutput = OutputFormat + "/Audio";
        private const string AnimClipOutput = OutputFormat + "/AnimClip";
        //Recorder Menus
        private const string MovieRecorderMenu = RecorderType + "/Movie";
        private const string AnimClipRecorderMenu = RecorderType + "/AnimClip";
        private const string ImageSeqRecorderMenu = RecorderType + "/ImageSequence";
        private const string AudioRecorderMenu = RecorderType + "/Audio";
        
        public static bool RecorderEnabled
        {
            get { return EditorPrefs.GetBool(SettingName); }
            set { EditorPrefs.SetBool(SettingName, value); }
        }
        public static int NumberCameraTags
        {
            get { return EditorPrefs.GetInt(NumberCamTagsFileName); }
            set { EditorPrefs.SetInt(NumberCamTagsFileName, value); }
        }
        //might not need this
        public static int NumberCamerasInScene
        {
            get { return EditorPrefs.GetInt(NumberCamerasSceneFileName); }
            set { EditorPrefs.SetInt(NumberCamerasSceneFileName, value); }
        }
        public static string TheRecorderSettingsJSON
        {
            get { return EditorPrefs.GetString(JSONEditorName); }
            set { EditorPrefs.SetString(JSONEditorName, value); }
        }
        public static bool HaveRecorderSettings
        {
            get { return EditorPrefs.GetBool(HaveSettingsFileName); }
            set { EditorPrefs.SetBool(HaveSettingsFileName, value); }
        }
        public static string TheCameraSettingsData
        {
            get { return EditorPrefs.GetString(CameraSettingsFileName); }
            set { EditorPrefs.SetString(CameraSettingsFileName, value); }
        }
        #endregion
        #region Tags & Setup

        [MenuItem(SetupAddTenCameras,priority = FP_UtilityData.ORDER_SUBMENU_LVL5)]
        static protected void AddTenCameraTags()
        {
            AddNumberTags(10,true);
        }
        [MenuItem(SetupAddFiveCameras, priority = FP_UtilityData.ORDER_SUBMENU_LVL5+1)]
        static protected void AddFiveCameraTags()
        {
            AddNumberTags(5,true);
        }
        #region Reset Menu Methods
        [MenuItem(RemoveAllFPCameras, priority =FP_UtilityData.ORDER_SUBMENU_LVL4+5)]
        static protected void RemoveAllCameraTagsMatchingPattern()
        {
            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            //string matchedTags = "Matched Tags:\n";
            List<string> _matchingTags = new List<string>();

            for(int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];
                if (tag.Contains(FP_RecorderUtility.CamTAG))
                {
                    _matchingTags.Add(tag);
                    Debug.Log($"Found a potential match: {tag}");
                }
            }
            for(int l = 0; l < _matchingTags.Count; l++)
            {
                FPGenerateTag.RemoveTag(_matchingTags[l]);
            }
            RecorderEnabled = false;
            NumberCameraTags = 0;
            
            Menu.SetChecked("FuzzPhyte/FP_Recorder/Ready", false);
            EditorUtility.DisplayDialog("Removed All Camera Tags",$"{_matchingTags.Count} Tags have been removed. Now there are {NumberCameraTags} FP_Record camera tags in scene" , "OK");
 
        }
        
        [MenuItem(ResetAllEditorData, priority = FP_UtilityData.ORDER_SUBMENU_LVL4 + 6)]
        static protected void ResetDataMenu()
        {
            settingsData = new FPRecorderSettingsJSON(RecordMode.SingleFrame, 1, true, -1, true);
            cameraData = new FPRecorderGOCams();
            TheCameraSettingsData = JsonUtility.ToJson(cameraData);
            NumberCamerasInScene = 0;
            //save the settings as json and store them to my editerprefs
            TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
            
            if(settings != null){
                Undo.RecordObject(settings, "Modify Recorder Settings");
            }
            settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            settings.ExitPlayMode = settingsData.ExitPlayMode;
            settings.CapFrameRate = settingsData.CapFPS;
            settings.FrameRate = settingsData.FrameRate;
            settings.FrameRatePlayback = settingsData.Playback;
            HaveRecorderSettings = true;
            Debug.LogWarning($"Reset the editor data");

            if (NumberCameraTags > 0)
            {
                RecorderEnabled = true;
                Menu.SetChecked("FuzzPhyte/FP_Recorder/Ready", true);
            }
            else
            {
                RecorderEnabled = false;
                Menu.SetChecked("FuzzPhyte/FP_Recorder/Ready", false);
            }
            
            CacheRecorderControllerSettings();
            EditorUtility.DisplayDialog("Reset Editor Pref Data!", $"Recorder Enabled? {RecorderEnabled}. Are there Tags? {NumberCameraTags} FP_Record camera tags in scene", "OK");
        
        }
        /// <summary>
        /// Function to save our RecorderControllerSettings file
        /// </summary> <summary>
        /// 
        /// </summary>
        static protected void CacheRecorderControllerSettings(){
             //
            // Define the folder path where the asset will be saved
            //AssetDatabase.CreateAsset(settings, "Assets/YourPath/RecorderSettings.asset");
           
            string folderPath = Path.Combine(FP_RecorderUtility.ReturnSamplesPath(),FP_RecorderUtility.CAT1);
            FP_Utility_Editor.CreateAssetPath(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            FP_Utility_Editor.CreatePackageSampleFolder(FP_RecorderUtility.PRODUCT_NAME_UNITY,FP_RecorderUtility.BASEVERSION);
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            string cleanPath = dataPath.Item2;
            cleanPath.Replace("\\","/");
            Debug.Log($"Asset Path DataPath?: {dataPath.Item2} vs clean path {cleanPath}");
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                var assetDatabaseLocation =AssetDatabase.CreateFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
                Debug.Log($"Asset Database Location: {assetDatabaseLocation}");
            }
            // The asset to be created
            //var asset = ScriptableObject.CreateInstance<FP_RecorderSO>();
            var fullPath = Path.Combine(dataPath.Item2,"FP_CachedRecorderControllerSettings.asset");
            if (string.IsNullOrEmpty(fullPath)) {
                Debug.LogError("Attempted to use an empty path to create an asset.");
                return; // Or handle the error appropriately
            }
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            AssetDatabase.CreateAsset(settings, assetPath);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        [MenuItem(ResetEVERYTHING, priority = FP_UtilityData.ORDER_SUBMENU_LVL4 + 7)]
        static protected void ResetEveryDataMenuItem()
        {
            RemoveAllCameraTagsMatchingPattern();
            ResetDataMenu();
        }
        #endregion
        /// <summary>
        /// Runtime add camera tag to make sure we stay synced 
        /// </summary>
        public static void RuntimeAddSingleTag()
        {
            NullCheckSettingsData();
            AddNumberTags(1,false);
        }
        
        /// <summary>
        /// Adds 'x' number of camera tags
        /// </summary>
        /// <param name="numberTags"></param>
        static protected void AddNumberTags(int numberTags, bool notifyMenu)
        {
            var startingCamNum = NumberCameraTags;
            var endCamValue = NumberCameraTags + numberTags;
            for (int i = startingCamNum; i < endCamValue; i++)
            {
                var camTag = FP_RecorderUtility.CamTAG + i.ToString();
                FPGenerateTag.CreateTag(camTag);
            }
            NumberCameraTags += numberTags;

            RecorderEnabled = true;

            //var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));

            if (notifyMenu)
            {
                Menu.SetChecked("FuzzPhyte/FP_Recorder/Ready", true);
                EditorUtility.DisplayDialog($"Added {numberTags} Camera Tags", $"'{FP_RecorderUtility.CamTAG}' with an '0,1,2,...' have been adedd! You now have {NumberCameraTags} FP_Record camera tags in the Project.", "OK");

            }

        }
        
        [MenuItem(SetupRemoveFiveCameras,priority =FP_UtilityData.ORDER_SUBMENU_LVL7+3)]
        static protected void RemoveFiveCameraTags()
        {
            RemoveNumberTags(5);
        }
        [MenuItem(SetupRemoveTenCameras,priority =FP_UtilityData.ORDER_SUBMENU_LVL7+2)]
        static protected void RemoveTenCameraTags()
        {
            RemoveNumberTags(10);
        }
        static protected void RemoveNumberTags(int numberTags)
        {
            var currentCamTags = NumberCameraTags;
            var endCamTags = NumberCameraTags - numberTags;
            for (int i = currentCamTags - 1; i >= endCamTags; i--)
            {
                var camTag = FP_RecorderUtility.CamTAG + i.ToString();
                FPGenerateTag.RemoveTag(camTag);
            }
            NumberCameraTags -= numberTags;
            if (NumberCameraTags <= 0)
            {
                NumberCameraTags = 0;
                RecorderEnabled = false;
                Menu.SetChecked("FuzzPhyte/FP_Recorder/Ready", false);
            }

            EditorUtility.DisplayDialog($"Removed {numberTags} Camera Tags", $"'{FP_RecorderUtility.CamTAG}' with an '0,1,2,...' have been Removed! You now have {NumberCameraTags} FP_Record camera tags in the Project.", "OK");

        }
        [MenuItem("FuzzPhyte/FP_Recorder/Ready",priority=FP_UtilityData.ORDER_SUBMENU_LVL6)]
        static protected void SetupChecked()
        {
            if (RecorderEnabled)
            {
                Debug.Log($"Camera Tags look good!");
            }
            else
            {
                Debug.Log($"You need to run the 'Setup' option in the FuzzPhyte Menu to generate the camera tags");
            }
        }
        #endregion

        #region Recorder Generation

        //JOHN MENU REMOVED ITEM [MenuItem(MovieRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL1)]
        static protected void GenerateMovieRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.Movie, dataPath.Item2, "AMovieRecorder.asset", "FP_Movie");
        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieRecorderMenu, true)]
        static bool CreateRecorderMovieActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(AnimClipRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL1+1)]
        static protected void GenerateAnimClipRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.AnimationClip, dataPath.Item2, "AAnimClipRecorder.asset","FP_AnimClip");
        }
        //JOHN MENU REMOVED ITEM [MenuItem(AnimClipRecorderMenu, true)]
        static bool CreateRecorderAnimClipActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(ImageSeqRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL1 + 2)]
        static protected void GenerateImageSeqRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.ImageSequence, dataPath.Item2, "ImageSequenceRecorder.asset","FP_ImgSequence");
        }
        //JOHN MENU REMOVED ITEM [MenuItem(ImageSeqRecorderMenu, true)]
        static bool CreateRecorderImageSeqActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(AudioRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL1 + 3)]
        static protected void GenerateAudioRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.Audio, dataPath.Item2, "AudioRecorder.asset","FP_Audio");
        }
        //JOHN MENU REMOVED ITEM [MenuItem(AudioRecorderMenu, true)]
        static protected bool CreateAudioRecorder()
        {
            return RecorderEnabled;
        }
        static protected void GenerateRecorder(FPRecorderType theType, string fulllocalPath, string assetName, string recorderName)
        {
            // The asset to be created
            var asset = FP_RecorderDataSO.CreateInstance(theType,recorderName);
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(fulllocalPath + "/"+assetName);
            // Create the asset
            CreateAssetAt(asset, assetPath);
        }

        #endregion

        #region Fake Menu Headers
        // FAKE HEADER
        [MenuItem(DateOutputHeader, false, FP_UtilityData.ORDER_SUBMENU_LVL4)]
        private static void FalseDataMenuHeader() { }

        // Ensure the header is non-functional
        [MenuItem(DateOutputHeader, true)]
        private static bool FalseDataMenuHeaderValidation() => false;

        [MenuItem(CreateOutputHeader, false, FP_UtilityData.ORDER_SUBMENU_LVL5)]
        private static void FalseCreateConfigMenuHeader() { }

        // Ensure the header is non-functional
        [MenuItem(CreateOutputHeader, true)]
        private static bool FalseCreateHeaderValidation() => false;
        #endregion
        #region Recorders OutputFormat


        //JOHN MENU REMOVED ITEM [MenuItem(AnimClipOutput, priority = FP_UtilityData.ORDER_SUBMENU_LVL1)]
        static protected void GenerateOutputAnimationClip()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);

            var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatAnimationClip.asset");

            //create the asset
            CreateAssetAt(asset, assetPath);

        }
        //JOHN MENU REMOVED ITEM [MenuItem(AnimClipOutput, true)]
        static bool OutputAnimationClipActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieOutputMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL1+1)]
        static protected void GenerateOutputMovieEncoderUnity()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);
            var CoreEncoderSettings = new UnityEditor.Recorder.Encoder.CoreEncoderSettings();
            CoreEncoderSettings.Codec = UnityEditor.Recorder.Encoder.CoreEncoderSettings.OutputCodec.MP4;
            CoreEncoderSettings.EncodingQuality = UnityEditor.Recorder.Encoder.CoreEncoderSettings.VideoEncodingQuality.High;
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderUnity.asset");
            var asset = MovieRecorderUnityMedia.CreateInstance(true, CoreEncoderSettings);
            CreateAssetAt(asset, assetPath);
        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieOutputMenu, true)]
        static bool OutputMovieEncoderUnityActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieOutputProResMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL1+2)]
        static protected void GenerateOutputMovieEncoderProRes()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);
            
            var ProResEncoderSettings = new UnityEditor.Recorder.Encoder.ProResEncoderSettings();
            ProResEncoderSettings.Format = UnityEditor.Recorder.Encoder.ProResEncoderSettings.OutputFormat.ProRes4444XQ;
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderProRes.asset");
            var asset = MovieRecorderProRes.CreateInstance(true, ProResEncoderSettings);
            
            CreateAssetAt(asset, assetPath);
        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieOutputProResMenu, true)]
        static bool OutputMovieEncoderProResActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieOutputGif, priority =FP_UtilityData.ORDER_SUBMENU_LVL1+3)]
        static protected void GenerateOutputMovieEncoderGif()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);
            var gifEncoder = new UnityEditor.Recorder.Encoder.GifEncoderSettings();
            gifEncoder.Loop = true;
            gifEncoder.Quality = 90;
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderGif.asset");
            var asset = MovieRecorderGif.CreateInstance(gifEncoder);
            CreateAssetAt(asset, assetPath);

        }
        //JOHN MENU REMOVED ITEM [MenuItem(MovieOutputGif, true)]
        static bool OutputMovieEncoderGifActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(ImageSeqOutput, priority = FP_UtilityData.ORDER_SUBMENU_LVL1+4)]
        static protected void GenerateOutputImageSequence()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatImgSequence.asset");
            var asset = ImageSeqRecorder.CreateInstance( ImageRecorderSettings.ImageRecorderOutputFormat.PNG , ImageRecorderSettings.EXRCompressionType.Zip, 100);
            CreateAssetAt(asset, assetPath);
        }
        //JOHN MENU REMOVED ITEM [MenuItem(ImageSeqOutput, true)]
        static bool OutputMovieEncoderImageSequenceActive()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(AudioOutput, priority = FP_UtilityData.ORDER_SUBMENU_LVL1+5)]
        static protected void GenerateOutputAudio()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatAudio.asset");
            var asset = AudioRecorder.CreateInstance();
            CreateAssetAt(asset, assetPath);
        }
        //JOHN MENU REMOVED ITEM [MenuItem(AudioOutput, true)]
        static bool OutputMovieAudioActive()
        {
            return RecorderEnabled;
        }
        #endregion

        #region Recorder InputFormat
        //JOHN MENU REMOVED ITEM [MenuItem(CamThreeSixtyInput, priority = FP_UtilityData.ORDER_SUBMENU_LVL1)]
        static protected void GenerateInputFileCamThreeSixty()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT2);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/InputFileCam360.asset");

            var asset = GenerateCamThreeSixty();
            CreateAssetAt(asset, assetPath);
        }
        //JOHN MENU REMOVED ITEM [MenuItem(CamThreeSixtyInput, true)]
        static bool InputFileCamThreeSixty()
        {
            return RecorderEnabled;
        }
        static protected ThreeSixtyRecordData GenerateCamThreeSixty()
        {
            var camSettings = new Camera360InputSettings();

            camSettings.CameraTag = FP_RecorderUtility.CamTAG + 0.ToString();
            camSettings.Source = UnityEditor.Recorder.ImageSource.TaggedCamera;
            camSettings.OutputWidth = 4096;
            camSettings.OutputHeight = 2048;
            camSettings.RenderStereo = false;
            camSettings.MapSize = 2048;
            return ThreeSixtyRecordData.CreateInstance(camSettings);
        }
        #endregion
        #region Populate GameObject in Hierarchy
        public static void RuntimeAddSingleCameraObject(GameObject runTimeOBJ)
        {
            GenerateGameObject(1,"FPCamera",FP_RecorderUtility.CamTAG,settingsData.RendererIndex);
        }
        [MenuItem(FiveCameraSpawnSettings,priority =FP_UtilityData.ORDER_SUBMENU_LVL5+2)]
        protected static void Generate5CameraObjects()
        {
            //check if we have the right amount of tags to manage this
            //we are going to add some cameras - when we do this we are going to increase the # we are tracking
            NullCheckSettingsData();
            GenerateGameObject(5, "FPCamera", FP_RecorderUtility.CamTAG,settingsData.RendererIndex);
            //we need to now back that number out to reset our starting number for the data files themselves
            var startingNumberTag = NumberCamerasInScene - 5;
            New360RecorderDataCount(5, FP_RecorderUtility.CamTAG,startingNumberTag);
        }
        public static void RunTimeAddSingleCamera(int tagNumber, string camTag)
        {
            //var newDataStruct = new FPRecorderDataStruct();
            var newList = new List<FPWildCards>
            {
                FPWildCards.SCENE,
                FPWildCards.RECORDER
            };
            var inputSettings = new Camera360InputSettings();
            var outputFormat = ScriptableObject.CreateInstance<ImageRecorderSettings>();

            //input
            inputSettings.OutputHeight = 2048;
            inputSettings.OutputWidth = 4096;
            inputSettings.MapSize = 2048;
            inputSettings.RenderStereo = false;
            inputSettings.FlipFinalOutput = false;
            inputSettings.Source = ImageSource.TaggedCamera;
            inputSettings.CameraTag = camTag;
            //output format
            outputFormat.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            outputFormat.EXRCompression = ImageRecorderSettings.EXRCompressionType.Zip;
            //output file & set files to the 
            var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            blankImageRecorder.imageInputSettings = inputSettings;
            blankImageRecorder.OutputFile = CreateOutputFileFromWildCards(newList).FileName;
            blankImageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;;
            blankImageRecorder.EXRCompression = ImageRecorderSettings.EXRCompressionType.Zip;
            blankImageRecorder.JpegQuality = 100;
            blankImageRecorder.name = "FP_360Setup_" +tagNumber;
            settings.AddRecorderSettings(blankImageRecorder);
            RuntimeUpdateSettingsFromPreserve();
            //New360RecorderDataCount(1,FP_RecorderUtility.CamTAG,tagNumber);
        }
        private static FP_OutputFileSO CreateOutputFileFromWildCards(List<FPWildCards> allWildCards)
        {
            var outputFileAsset = FP_OutputFileSO.CreateInstance(allWildCards, UnityEditor.Recorder.OutputPath.Root.AssetsFolder);
            return outputFileAsset;
        }
        [MenuItem(TenCameraSpawnSettings,priority = FP_UtilityData.ORDER_SUBMENU_LVL5 + 3)]
        protected static void Generate10CameraObjects()
        {
            //check if we have the right amount of tags to manage this
            //we are going to add some cameras - when we do this we are going to increase the # we are tracking
            NullCheckSettingsData();
            GenerateGameObject(10, "FPCamera", FP_RecorderUtility.CamTAG, settingsData.RendererIndex);
            //we need to now back that number out to reset our starting number for the data files themselves
            var startingNumberTag = NumberCamerasInScene - 10;
            New360RecorderDataCount(10, FP_RecorderUtility.CamTAG, startingNumberTag);
        }
        private static void NullCheckSettingsData()
        {
            if(settingsData == null)
            {
                //can we get the data from the editor prefs via settings data?
                var passedSettings = GetCurrentRecorderSettings();

                settingsData = new FPRecorderSettingsJSON(RecordMode.SingleFrame, 1, true, -1, true);
                NumberCamerasInScene = 0;
                NumberCameraTags = 0;
                Debug.LogWarning($"Establishing Editor Prefs for FP_Recorder");
            }
        }
        /// <summary>
        /// Generate GameObjects from a list of items from data- JSON
        /// </summary>
        /// <param name="transformData"></param>
        /// <param name="baseName"></param>
        static void GenerateGameObjectFromDataList(List<FPTransformStruct>transformData,string baseName, int rendererIndex=-1)
        {
            var currentCamsInScene = NumberCamerasInScene;
            var endCamsInScene = NumberCamerasInScene + transformData.Count;
            var numberToSpawn = transformData.Count;
            if (NumberCameraTags < endCamsInScene)
            {
                //we need more tags
                Debug.Log($"We need more tags!");
                AddNumberTags(numberToSpawn, false);

            }
            GameObject newParent = new GameObject();
            newParent.name = baseName + "_" + numberToSpawn + "_cameraParent";
            newParent.transform.position = new Vector3(0, 0, 0);
            //
            if (cameraData == null)
            {
                cameraData = new FPRecorderGOCams();
            }
            for (int i = 0; i < numberToSpawn; i++)
            {
                GameObject newObj = new GameObject();
                var dataObj = transformData[i];

                newObj.name = dataObj.Name;
                cameraData.GameObjectNames.Add(newObj.name);


                newObj.tag = dataObj.Tag; // Assign the tag
                //assign position
                newObj.transform.position = dataObj.Position;
                newObj.transform.rotation = Quaternion.Euler(dataObj.Rotation);
                newObj.transform.SetParent(newParent.transform);
                //newObj.transform.localPosition = new Vector3(0, yStartHeight, (i + 1) * 0.5f);
                Camera newCam = newObj.AddComponent<Camera>(); // Add Camera component
                                                               //set renderer here
                
#if UNITY_PIPELINE_URP
                var camData = newCam.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                if(camData!= null)
                {
                    camData.SetRenderer(rendererIndex);
                }   
                //camData.SetRenderer(rendererIndex);
#endif
                if(newObj.GetComponent<AudioListener>() != null)
                {
                    UnityEngine.Object.DestroyImmediate(newObj.GetComponent<AudioListener>()); // Remove the Audio Listener
                }
               

            }
            NumberCamerasInScene += numberToSpawn;
            //sync EditorPrefs cam data
            TheCameraSettingsData = JsonUtility.ToJson(cameraData);
            //select the parent item
            Selection.activeGameObject = newParent;
            //
        }
        /// <summary>
        /// Generate GameObjects from runtime that is coming in From FPPreserveObject.cs 
        /// </summary>
        /// <param name="runtimeCams"></param>
        public static void GenerateGameObjectRuntime(Transform PassedParent,List<GameObject> runtimeCams,string baseName, string baseTag)
        {
            if(runtimeCams.Count<=0){
                return;
            }
            var camIndex = settingsData.RendererIndex;
            var numberToSpawn = runtimeCams.Count;
            var currentCamsInScene = NumberCamerasInScene;
            Debug.Log($"Current # FP Cams in Scene: {currentCamsInScene}, current Number to spawn {numberToSpawn}");
            var endCamsInScene = NumberCamerasInScene + numberToSpawn;
            if (NumberCameraTags < endCamsInScene)
            {
                //we need more tags
                Debug.Log($"We need more tags!");
                AddNumberTags(numberToSpawn,false);
            }
            /*
            GameObject preservedObject = PrefabUtility.InstantiatePrefab(obj) as GameObject;

            // Explicitly copy transform properties
            if (preservedObject != null)
            {
                preservedObject.transform.position = obj.transform.position;
                preservedObject.transform.rotation = obj.transform.rotation;
                preservedObject.transform.localScale = obj.transform.localScale;
            }
            */
            GameObject newParent = GameObject.Instantiate(PassedParent.gameObject);
            newParent.name = baseName + "_" + numberToSpawn + "_cameraParent";
            newParent.transform.position = new Vector3(0, 0, 0);
        
            if (cameraData == null)
            {
                cameraData = new FPRecorderGOCams();
            }
            for(int i = 0; i < numberToSpawn; i++)
            {
                var passedGO = runtimeCams[i];
                GameObject newObj = GameObject.Instantiate(runtimeCams[i]) as GameObject;
                int camTagNumber = currentCamsInScene + i;
                newObj.name = baseName +"_"+i.ToString()+ "_" + camTagNumber.ToString(); // Unique name using GUID
                cameraData.GameObjectNames.Add(newObj.name);
                

                newObj.tag = baseTag + camTagNumber.ToString(); // Assign the tag
                newObj.transform.SetParent(newParent.transform);
                newObj.transform.localPosition = passedGO.transform.position;
                Camera newCam = newObj.AddComponent<Camera>(); // Add Camera component
                                                            //update the renderer index

#if UNITY_PIPELINE_URP
                var camData = newCam.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                if (camData != null)
                {
                    camData.SetRenderer(camIndex);
                }
#endif
                UnityEngine.Object.DestroyImmediate(newObj.GetComponent<AudioListener>()); // Remove the Audio Listener
            }
            NumberCamerasInScene += numberToSpawn;
            //sync EditorPrefs cam data
            TheCameraSettingsData = JsonUtility.ToJson(cameraData);
            //select the parent item
            Selection.activeGameObject = newParent;
        }
        public static void GenerateGameObjectRunTime()
        {
            if(cameraData == null)
            {
                cameraData = new FPRecorderGOCams();
            }
        }
        public static void AddGameObjectCameraData(string objName)
        {
            cameraData.GameObjectNames.Add(objName);
        }
        /// <summary>
        /// Drop in GameObjects for cameras
        /// </summary>
        /// <param name="numberToSpawn"></param>
        /// <param name="baseName"></param>
        /// <param name="baseTag"></param>
        /// <param name="yStartHeight"></param>
        static void GenerateGameObject(int numberToSpawn,string baseName, string baseTag, int camIndex,float yStartHeight=1.7f)
        {
            var currentCamsInScene = NumberCamerasInScene;
            Debug.Log($"Current # FP Cams in Scene: {currentCamsInScene}");
            var endCamsInScene = NumberCamerasInScene + numberToSpawn;
            if (NumberCameraTags < endCamsInScene)
            {
                //we need more tags
                Debug.Log($"We need more tags!");
                AddNumberTags(numberToSpawn,false);
                
            }
        
            GameObject newParent = new GameObject();
            newParent.name = baseName + "_" + numberToSpawn + "_cameraParent";
            newParent.transform.position = new Vector3(0, 0, 0);
        
            if (cameraData == null)
            {
                cameraData = new FPRecorderGOCams();
            }
            for(int i = 0; i < numberToSpawn; i++)
            {
                GameObject newObj = new GameObject();
                int camTagNumber = currentCamsInScene + i;
                newObj.name = baseName +"_"+i.ToString()+ "_" + camTagNumber.ToString(); // Unique name using GUID
                cameraData.GameObjectNames.Add(newObj.name);
                

                newObj.tag = baseTag + camTagNumber.ToString(); // Assign the tag
                newObj.transform.SetParent(newParent.transform);
                newObj.transform.localPosition = new Vector3(0, yStartHeight, (i+1)*0.5f);
                Camera newCam = newObj.AddComponent<Camera>(); // Add Camera component
                                                            //update the renderer index

#if UNITY_PIPELINE_URP
                var camData = newCam.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                if (camData != null)
                {
                    camData.SetRenderer(camIndex);
                }
                
#endif
                UnityEngine.Object.DestroyImmediate(newObj.GetComponent<AudioListener>()); // Remove the Audio Listener
            }
            NumberCamerasInScene += numberToSpawn;
            //sync EditorPrefs cam data
            TheCameraSettingsData = JsonUtility.ToJson(cameraData);
            //select the parent item
            Selection.activeGameObject = newParent;
            
        }
        /// <summary>
        /// Generate Recorder Data by Number of Cameras Needed
        /// </summary>
        /// <param name="numRecorders"></param>
        /// <param name="startTag"></param>
        static protected void New360RecorderDataCount(int numRecorders, string startTag, int startTagNum)
        {
            var newList = new List<FPWildCards>
            {
                FPWildCards.SCENE,
                FPWildCards.RECORDER
            };
            NullCheckSettingsData();
            for (int i = 0; i < numRecorders; i++)
            {
                var curTagValue = startTagNum + i;
                
                var dataItem = settingsData.CreateImageSequence360PNG(2048, 4096,2048, ImageRecorderSettings.ImageRecorderOutputFormat.PNG, ImageSource.TaggedCamera, ImageRecorderSettings.EXRCompressionType.Zip, newList, startTag + curTagValue.ToString());
                dataItem.RecorderName = "FP_360Setup_" + curTagValue.ToString();
                //update name by one value
                settingsData.RecorderData.Add(dataItem);
                //spawn a gameobject camera
            }



            //need to back up my data
            TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
            Debug.LogWarning($"JSON DUMP| \n {TheRecorderSettingsJSON}");
            //now lets use our data class and populate the entire recorder
            AddFPRecorderDataToRecorderWindow();
        }
#endregion
        #region New Data Menu Items
        /// <summary>
        /// Checks to see if we have recorder settings
        /// if we do we try to load it and serialize/deserialize it from that JSON format
        /// </summary>
        [MenuItem(LoadRecorderFromData, priority = FP_UtilityData.ORDER_SUBMENU_LVL7 + 8)]
        protected static void HaveJSONRecorderSettings()
        {
            if (HaveRecorderSettings)
            {
                var anyErrors = ReadEditorPrefsJSON();

                if (!anyErrors)
                {
                    var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
                    //someWindow.
                    someWindow.SetRecorderControllerSettings(settings);
                }
                Debug.LogWarning($"Settings Data Outcome after FileOnBoot {settingsData.RecorderNotes}");
                
            }
            else
            {
                Debug.LogWarning($"No Settings file in the editorPrefs");
                //need to generate my settingsData file
                //single Frame Mode as a generic first one
                settingsData = new FPRecorderSettingsJSON(RecordMode.SingleFrame, 1, true, -1, true);
                cameraData = new FPRecorderGOCams();
                //need to generate a blank 
                //save the settings as json and store them to my editerprefs
                TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
                if(settings != null){
                    Undo.RecordObject(settings, "Modify Recorder Settings");
                }
                settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                TheCameraSettingsData = JsonUtility.ToJson(cameraData);
                settings.ExitPlayMode = settingsData.ExitPlayMode;
                settings.CapFrameRate = settingsData.CapFPS;
                settings.FrameRate = settingsData.FrameRate;
                settings.FrameRatePlayback = settingsData.Playback;
                CacheRecorderControllerSettings();
                HaveRecorderSettings = true;
            }

        }
        /// <summary>
        /// Works around settingsData requirements
        /// Uses the EditerPrefs to load 'TheRecorderSettingsJSON'
        /// </summary>
        /// <returns></returns>
        static protected bool ReadEditorPrefsJSON()
        {
            Debug.Log($"String message that should be maybe JSON BEGINS HERE| {TheRecorderSettingsJSON}");
            //need to bring the file into my data
            settingsData = JsonUtility.FromJson<FPRecorderSettingsJSON>(TheRecorderSettingsJSON);
            Debug.LogWarning($"Found that we might have some settings in the EditorPrefs, but we need to load them from the JSON");
            string returnMessages = "";
            Debug.LogWarning($"Settings Data Notes Before: {settingsData.RecorderNotes}");
            bool anyErrors = false;
            try
            {
                var settingsDataReturn = settingsData.GenerateRecorderControllerSettingsForUnity(out returnMessages);
                Debug.LogWarning($"Messages Returned:{returnMessages}");
                if (settingsDataReturn.Item2)
                {
                    settings = settingsDataReturn.Item1;
                    
                    //try to load it into the editor
                    Debug.LogWarning($"File appears loaded correctly. Going to try and populate the recoder!");

                }
                else
                {
                    Debug.LogWarning($"Our Settings File contained not enough information, need to manually build it and/or look into something...");
                    if(settings != null){
                        Undo.RecordObject(settings, "Modify Recorder Settings");
                    }
                    settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                    settings.ExitPlayMode = settingsData.ExitPlayMode;
                    settings.CapFrameRate = settingsData.CapFPS;
                    settings.FrameRate = settingsData.FrameRate;
                    settings.FrameRatePlayback = settingsData.Playback;
                    if (settingsDataReturn.Item1 != null)
                    {
                        settings = settingsDataReturn.Item1;
                        Debug.LogWarning($"Rebuilt the file: assigned settings to the data we passed back, going to try and populate the recoder!");

                    }
                    else
                    {
                        Debug.LogWarning($"Return was null!");
                    }
                    CacheRecorderControllerSettings();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error: {ex.Message}| Running 'SetupSettingsFileOnBoot");
                anyErrors = true;
            }
            return anyErrors;
        }
        public static void RuntimeUpdateSettingsFromPreserve()
        {
            var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
            someWindow.SetRecorderControllerSettings(settings);
        }
        
        //JOHN MENU REMOVED ITEM [MenuItem(CreateAndAdd360RecorderToRecorderSettings, priority = FP_UtilityData.ORDER_SUBMENU_LVL1+6)]
        static protected void CreateAndAddNew360RecorderDataElement()
        {
            //use existing data element to generate new element
            if (settingsData != null)
            {
                var newList = new List<FPWildCards>
                {
                    FPWildCards.SCENE,
                    FPWildCards.RECORDER
                };

                var dataItem = settingsData.CreateImageSequence360PNG(2048, 4096,2048, ImageRecorderSettings.ImageRecorderOutputFormat.PNG, ImageSource.TaggedCamera,ImageRecorderSettings.EXRCompressionType.Zip, newList, FP_RecorderUtility.CamTAG);
                dataItem.RecorderName = "FP_360Setup" + "_"+(settingsData.RecorderData.Count+1).ToString();
                //update name by one value
                
                settingsData.RecorderData.Add(dataItem);
                //need to back up my data
                TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
                Debug.LogWarning($"JSON DUMP| \n {TheRecorderSettingsJSON}");
                //now lets use our data class and populate the entire recorder
                AddFPRecorderDataToRecorderWindow();

            }
            else
            {
                Debug.LogError($"Missing a reference to settingsData!");
            }
        }
        
        /// <summary>
        /// Adds our data to the actual recorder window
        /// </summary>
        static protected void AddFPRecorderDataToRecorderWindow()
        {
            //going from settingsData to settings to then the window
            //need to probably clear it and add it in from data?
            string outMessages = "";
            var settingsReturn = settingsData.GenerateRecorderControllerSettingsForUnity(out outMessages);
            Debug.LogWarning($"Messages returned: {outMessages}");
            if (settingsReturn.Item2)
            {
                settings = settingsReturn.Item1;
            }
            else
            {
                Debug.LogWarning($"Settings isn't exactly right, need to rebuild it?");
                if(settings != null){
                    Undo.RecordObject(settings, "Modify Recorder Settings");
                }
                settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                settings.ExitPlayMode = settingsData.ExitPlayMode;
                settings.CapFrameRate = settingsData.CapFPS;
                settings.FrameRate = settingsData.FrameRate;
                settings.FrameRatePlayback = settingsData.Playback;
                settings = settingsReturn.Item1;
                CacheRecorderControllerSettings();
            }
            var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
            //someWindow.
            someWindow.SetRecorderControllerSettings(settings);
        }
        //Save items from the editor via the tags
        static protected void SaveCameraPositions()
        {
            //use the tags as guidance
            //FP_RecorderUtility.CamTAG
            //    NumberCameraTags
            for (int i = 0; i < NumberCameraTags; i++)
            {

                var searchTag = FP_RecorderUtility.CamTAG + i.ToString();
                Debug.Log($"Searching for a camera with tag {searchTag}");
                var allFoundItem = GameObject.FindGameObjectWithTag(searchTag);
                if (allFoundItem != null)
                {
                    //if it has a camera on it
                    //throw it at my list
                    
                    Debug.Log($"Found it, {allFoundItem.name} with location at {allFoundItem.transform.position.x},{allFoundItem.transform.position.y},{allFoundItem.transform.position.z}");
                    var dataBack = settingsData.AddCameraData(allFoundItem);
                    Debug.Log($"Results of adding it to data?: {dataBack}");
                }
            }
            //need to back up my data
            TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
        }
        /// <summary>
        /// Runtime Call from the FPRecorderCam
        /// </summary>
        public static void RunTimeSaveJSON()
        {
            WriteJSONToLocalFile();
        }
        /// <summary>
        /// Called from the FPRecorderCam When we Hit the Button
        /// </summary>
        public static void RuntimeResetSettings()
        {
             Debug.LogWarning($"Reset the Settings file in the editorPrefs");
            //need to generate my settingsData file
            //single Frame Mode as a generic first one
            settingsData = new FPRecorderSettingsJSON(RecordMode.SingleFrame, 1, true, -1, true);
            cameraData = new FPRecorderGOCams();
            //need to generate a blank 
            //save the settings as json and store them to my editerprefs
            TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
            settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            TheCameraSettingsData = JsonUtility.ToJson(cameraData);
            settings.ExitPlayMode = settingsData.ExitPlayMode;
            settings.CapFrameRate = settingsData.CapFPS;
            settings.FrameRate = settingsData.FrameRate;
            settings.FrameRatePlayback = settingsData.Playback;
            CacheRecorderControllerSettings();
            HaveRecorderSettings = true;
        }
        public static async void RuntimeWriteRecorderAsset(string userFileName)
        {
            var localSamplesFolder = await FP_RecorderUtility.ReturnInstallPath();
            (bool,string) dataPath=(false,"");
            try
            {
                var potentialFolder = Path.Combine(Application.dataPath, localSamplesFolder);
                var fileIO = File.Exists(potentialFolder);
                Debug.Log($"Local Samples Folder: {potentialFolder}, exist? {fileIO}");
                //check if a folder exists for the data
                if (!fileIO)
                {
                    Directory.CreateDirectory(potentialFolder);
                }
              
                if(File.Exists(Path.Combine(potentialFolder, FP_RecorderUtility.SAMPLELOCALFOLDER)))
                {
                    Debug.Log($"Local Samples Folder: {FP_RecorderUtility.SAMPLELOCALFOLDER} exists!");
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(potentialFolder, FP_RecorderUtility.SAMPLELOCALFOLDER));
                }
                Debug.Log($"Before Assets: {localSamplesFolder}");
                localSamplesFolder = Path.Combine("Assets", localSamplesFolder);
                Debug.Log($"After Assets: {localSamplesFolder}");
                AssetDatabase.Refresh();
                dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(localSamplesFolder, FP_RecorderUtility.SAMPLELOCALFOLDER);
                if (!dataPath.Item1)
                {
                    Debug.LogError($"Error on Path Creation, check the logs! Path at: {dataPath.Item2}");
                    return;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error on Creating Asset Folder via FP_Utility_Editor.CreateAssetDatabaseFolder() Message: {ex.Message}");
            }
            string currentSceneName = SceneManager.GetActiveScene().name;
            string backUpRootFile = "/FPBackup_" + userFileName+"_"+currentSceneName+"_"+System.DateTime.Now.ToString("MM_dd_yy_HHmm_s");
            string assetSettingsFile = backUpRootFile + "_settings.asset";
            SaveRecorderListFromUnityWindow(dataPath.Item2,assetSettingsFile);
        }
        [MenuItem(WriteBackupJSON, priority = FP_UtilityData.ORDER_SUBMENU_LVL5 + 9)]
        static protected async void WriteJSONToLocalFile()
        {
            var localSamplesFolder = await FP_RecorderUtility.ReturnInstallPath();
            (bool,string) dataPath=(false,"");
            try
            {
                var potentialFolder = Path.Combine(Application.dataPath, localSamplesFolder);
                var fileIO = File.Exists(potentialFolder);
                Debug.Log($"Local Samples Folder: {potentialFolder}, exist? {fileIO}");
                //check if a folder exists for the data
                if (!fileIO)
                {
                    Directory.CreateDirectory(potentialFolder);
                }
              
                if(File.Exists(Path.Combine(potentialFolder, FP_RecorderUtility.SAMPLELOCALFOLDER)))
                {
                    Debug.Log($"Local Samples Folder: {FP_RecorderUtility.SAMPLELOCALFOLDER} exists!");
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(potentialFolder, FP_RecorderUtility.SAMPLELOCALFOLDER));
                }
                Debug.Log($"Before Assets: {localSamplesFolder}");
                localSamplesFolder = Path.Combine("Assets", localSamplesFolder);
                Debug.Log($"After Assets: {localSamplesFolder}");
                AssetDatabase.Refresh();
                dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(localSamplesFolder, FP_RecorderUtility.SAMPLELOCALFOLDER);
                if (!dataPath.Item1)
                {
                    Debug.LogError($"Error on Path Creation, check the logs! Path at: {dataPath.Item2}");
                    return;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error on Creating Asset Folder via FP_Utility_Editor.CreateAssetDatabaseFolder() Message: {ex.Message}");
            }
            
            //sync my camera data
            SaveCameraPositions();
            //save my data
            TheRecorderSettingsJSON = JsonUtility.ToJson(settingsData);
            var jsonAsset = TheRecorderSettingsJSON;
            //var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
            string currentSceneName = SceneManager.GetActiveScene().name;
            string backUpRootFile = "/FPBackup_" + currentSceneName+"_"+System.DateTime.Now.ToString("MM_dd_yy_HHmm_s");
            
            string backupFile = backUpRootFile + ".json";
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2+backupFile);
            Debug.Log($"Asset Backup Path: {assetPath}");
            //FileStream file = File.Create(assetPath);
            //create the asset
            try
            {
                // Create a FileStream to write to the file, it will create the file if it does not exist, or overwrite it if it does.
                using (FileStream fileStream = new FileStream(assetPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    // Create a StreamWriter to write text to the file, using UTF8 encoding
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        // Write text to the file
                        writer.Write(jsonAsset);
                    } // StreamWriter is automatically flushed and closed here
                } // FileStream is automatically closed here

                Debug.Log($"Backup file generated at {assetPath} lets try generating and saving a 'Unity Recorder List'");
                
                //
            }
            catch (Exception ex)
            {
                // If an error occurs, display the message
               Debug.LogError("An error occurred while writing the backup file: " + ex.Message);
            }
            string assetSettingsFile = backUpRootFile + "_settings.asset";
            SaveRecorderListFromUnityWindow(dataPath.Item2,assetSettingsFile);
            
        }
        public static RecorderControllerSettings GetCurrentRecorderSettings()
        {
            // This line attempts to get the currently open Recorder window.
            // Note: The actual class name might differ and might not be directly accessible without reflection.
            var recorderWindow = EditorWindow.GetWindow<RecorderWindow>("Unity Recorder", typeof(RecorderWindow));

            if (recorderWindow != null)
            {
                // Access the RecorderController or its settings here
                // This part is speculative and depends on the internal implementation of the RecorderWindow
                // You may need to use reflection to get private fields if they are not publicly exposed
                var controller = recorderWindow.GetType().GetField("m_RecorderController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(recorderWindow) as RecorderController;

                if (controller != null)
                {
                    return controller.Settings;
                }
            }
            return null;
        }
        public static void ForceSettingsUpdate(RecorderControllerSettings passedSettings)
        {
            var runningList = passedSettings.RecorderSettings.ToList();
            //reset settings
            settings = null;
            settings = passedSettings;
            for(int i=0;i<runningList.Count;i++)
            {
                settings.AddRecorderSettings(runningList[i]);
            }
            Debug.Log($"Settings now should have: {settings.RecorderSettings.Count()} recorders");
            //settings.AddRecorderSettings()
            //settingsData = new FPRecorderSettingsJSON(settings);
        }
        public static void LoadRecorderSettings(RecorderControllerSettings loadSettings)
        {
            // This line attempts to get the currently open Recorder window.
            // Note: The actual class name might differ and might not be directly accessible without reflection.
            var recorderWindow = EditorWindow.GetWindow<RecorderWindow>("Unity Recorder", typeof(RecorderWindow));

            if (recorderWindow != null)
            {
                recorderWindow.SetRecorderControllerSettings(loadSettings);
            }
        }
        static protected void SaveRecorderListFromUnityWindow(string path, string fileName)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(path + fileName);
            var recorderWindow = EditorWindow.GetWindow<RecorderWindow>("Unity Recorder", typeof(RecorderWindow));
            if(settings != null)
            {
                RecorderControllerSettingsPreset.SaveAtPath(settings, assetPath);
                if(recorderWindow!= null){
                    recorderWindow.SaveChanges();
                    LoadRecorderSettings(settings);
                }
            }else{
                Debug.Log($"Settings are null, going to pull from the current recorder window then!");
                //get settings from the recorder 
                var activeSettings = GetCurrentRecorderSettings();
                if(activeSettings != null)
                {
                    Debug.Log($"Saved Settings from the Recorder Window on your behalf at {assetPath}");
                    RecorderControllerSettingsPreset.SaveAtPath(activeSettings, assetPath);
                    if(recorderWindow!= null){
                        recorderWindow.SaveChanges();
                        LoadRecorderSettings(settings);
                    }
                }
            }
        }
        [MenuItem(ReadBackupJSON, priority = FP_UtilityData.ORDER_SUBMENU_LVL5 + 8)]
        static protected void ReadJSONFromLocalSelectedFileMenu()
        {
            var data = Selection.activeObject as TextAsset;

            if (data != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(data.GetInstanceID());
                //find path of selected object
                try
                {
                    string content = string.Empty;
                    using (FileStream fileStream = new FileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            // Read the entire file content
                            content = reader.ReadToEnd();
                        } 
                    } 
                    Debug.Log($"Found the backup file and read it in from {assetPath}.");
                    //try now reading it back into Unity?
                    TheRecorderSettingsJSON = content;
                    var anyErrors = ReadEditorPrefsJSON();
                    //setup our tags to match if we need more
                    
                    if (!anyErrors)
                    {

                        HaveRecorderSettings = true;
                        //match tags
                        var curCountSize = settings.RecorderSettings.Count();
                        Debug.Log($"Current # of Recorders: {curCountSize}");
                        if (NumberCameraTags<curCountSize)
                        {
                            //increase number camera tags
                            var diff = curCountSize - NumberCameraTags;
                            if (diff > 0)
                            {
                                Debug.Log($"Adding more Tags as it looks like we need {diff} additional tags based on the data we are loading");
                                AddNumberTags(diff, false);
                            }
                        }
                        //we need to now spawn the gameobjects based on this data
                        GenerateGameObjectFromDataList(settingsData.CameraPlacements, FP_RecorderUtility.BaseCamName,settingsData.RendererIndex);
                        //GenerateGameObject(curCountSize, FP_RecorderUtility.BaseCamName, FP_RecorderUtility.CamTAG);
                        
                        var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
                        //someWindow.
                        someWindow.SetRecorderControllerSettings(settings);
                    }
                }
                catch (Exception ex)
                {
                    // If an error occurs, display the message
                    Debug.LogError($"An error occurred while attempting to read the backup file at {assetPath}: " + ex.Message);
                }

                //var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.BACKUP);
                //var jsonAsset = TheRecorderSettingsJSON;
                //var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
                //string backupFile = "/FPBackup_" + System.DateTime.Now.ToString("yyyyMMdd_hhmm") + ".json";
                //string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + backupFile);
            }
            else
            {
                Debug.LogError($"Make sure you have a TextAsset selected-when loading from the backup!");
            }
        }
        #endregion
        //JOHN MENU REMOVED ITEM [MenuItem(RecordTakeOutputFile, priority =FP_UtilityData.ORDER_SUBMENU_LVL1)]
        static protected void GenerateOutputFileSceneTake()
        {
            var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT4);
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFileRecordTake.asset");
            List<FPWildCards> _cards = new List<FPWildCards>
            {
                FPWildCards.SCENE,
                FPWildCards.RECORDER
            };
            var asset = FP_OutputFileSO.CreateInstance(_cards, UnityEditor.Recorder.OutputPath.Root.AssetsFolder);
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(RecordTakeOutputFile, true)]
        static bool OutputFileRecorderTake()
        {
            return RecorderEnabled;
        }
        //JOHN MENU REMOVED ITEM [MenuItem(CGenericConfigMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL3+1)]
        static protected void GenerateRecorderConfiguration()
        {
            // Define the folder path where the asset will be saved
            string folderPath = Path.Combine(FP_RecorderUtility.ReturnSamplesPath(),FP_RecorderUtility.CAT1);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            }

            // The asset to be created
            var asset = ScriptableObject.CreateInstance<FP_RecorderSO>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/ARecorderConfiguration.asset");

            // Create the asset
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            // Focus the asset in the Unity Editor
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            // Optionally, log the creation
            Debug.Log("ExampleAsset created at " + assetPath);
        }
        //JOHN MENU REMOVED ITEM [MenuItem(C360ConfigMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL3)]
        static protected void Generate360Configuration()
        {
            // Define the folder path where the asset will be saved
            //need to generate all of the supporting files first

            string folderPath = Path.Combine(FP_RecorderUtility.ReturnSamplesPath(),FP_RecorderUtility.CAT1);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT1);
            }

            // The asset to be created
            var recorderAsset = ScriptableObject.CreateInstance<FP_RecorderDataSO>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/A360Configuration.asset");
            recorderAsset.RecorderType = FPRecorderType.ImageSequence;
            recorderAsset.RecorderName = "FP_Generated_ImgSeq";
            //Create all of the other files we need for 360
            //ThreeSixtyRecordData:FP_InputDataSO
            var inputAsset = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT2);
            string inputAssetPath = AssetDatabase.GenerateUniqueAssetPath(inputAsset.Item2 + "/InputFileCam360.asset");
            //outputPath File
            var outputFile = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT4);
            var outputFileAssetPath = AssetDatabase.GenerateUniqueAssetPath(outputFile.Item2 + "/OutputFileRecordTake.asset");
            List<FPWildCards> _cards = new List<FPWildCards>
            {
                FPWildCards.SCENE,
                FPWildCards.RECORDER
            };
            var outputFileAsset = FP_OutputFileSO.CreateInstance(_cards, UnityEditor.Recorder.OutputPath.Root.AssetsFolder);
            CreateAssetAt(outputFileAsset, outputFileAssetPath);
            recorderAsset.OutputCameraData = outputFileAsset;
            //
            var inputCamAsset  = GenerateCamThreeSixty();
            CreateAssetAt(inputCamAsset, inputAssetPath);
            recorderAsset.InputFormatData = inputCamAsset;
            //FP_OutputFileSO
            //FP_OutputFormatSO - would be ImageSeqRecorder for generic 360
            var outputAsset = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.ReturnSamplesPath(), FP_RecorderUtility.CAT3);
            string imgSeqOutputAssetPath = AssetDatabase.GenerateUniqueAssetPath(outputAsset.Item2 + "/OutputFormatImgSequence.asset");

            var imgSeqAsset = ImageSeqRecorder.CreateInstance( ImageRecorderSettings.ImageRecorderOutputFormat.PNG, ImageRecorderSettings.EXRCompressionType.Zip, 100);
            CreateAssetAt(imgSeqAsset, imgSeqOutputAssetPath);
            recorderAsset.OutputFormatData = imgSeqAsset;

            //Create the Configuration Asset after the other 3 are done
            CreateAssetAt(recorderAsset, assetPath);
            // Focus the asset in the Unity Editor
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = recorderAsset;

            // Optionally, log the creation
            Debug.Log("ExampleAsset created at " + assetPath);
        }
        #region Scriptable Object Calls 
        public static void PassFullConfigurationFileFromSO(FP_RecorderDataSO passedData)
        {
            Debug.LogWarning($"This isn't fully setup yet! Number cameras passed: {passedData.NumberOfCameras}");
            //OLD SETUP
            var data = passedData;
            if (settings == null)
            {
                Debug.LogError($"Setting was null!");
                return;
            }
            //JOHN THIS RETURN
            //need to evaluate settings vs settingsData
            return;
            /*
             * GenerateGameObject(5, "FPCamera", FP_RecorderUtility.CamTAG);
            //we need to now back that number out to reset our starting number for the data files themselves
            var startingNumberTag = NumberCamerasInScene - 5;
            New360RecorderDataCount(5, FP_RecorderUtility.CamTAG,startingNumberTag);
             **/
            if (data != null)
            {
                //figure out what recorder to add based on the FP_RecorderDataSO
                var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                var movieImageRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
                var blankAudioRecorder = ScriptableObject.CreateInstance<AudioRecorderSettings>();
                var blankAnimClipRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
                //Input Settings first
                if (data.RecorderType == FPRecorderType.ImageSequence || data.RecorderType == FPRecorderType.Movie)
                {
                    blankImageRecorder.imageInputSettings = MovieImageSequenceInputTarget(data);
                    movieImageRecorder.ImageInputSettings = MovieImageSequenceInputTarget(data);
                    blankImageRecorder.OutputFile = data.OutputCameraData.FileName;
                    movieImageRecorder.OutputFile = data.OutputCameraData.FileName;
                    blankImageRecorder.name = data.RecorderName;
                    movieImageRecorder.name = data.RecorderName;
                }
                else
                {
                    if (data.RecorderType == FPRecorderType.Audio)
                    {
                        var castAudio = data.InputFormatData as AudioRecorder;
                        blankAudioRecorder = castAudio.ReturnUnityOutputFormat();
                        blankAudioRecorder.OutputFile = data.OutputCameraData.FileName;
                        blankAudioRecorder.name = data.RecorderName;
                    }
                    else
                    {
                        if (data.RecorderType == FPRecorderType.AnimationClip)
                        {
                            var castClip = data.InputFormatData as AnimationClipRecorder;
                            blankAnimClipRecorder = castClip.ReturnUnityOutputFormat(data.AnimationClipGameObject);
                            blankAnimClipRecorder.OutputFile = data.OutputCameraData.FileName;
                            blankAnimClipRecorder.name = data.RecorderName;
                        }
                    }
                }

                //output format now
                switch (data.RecorderType)
                {
                    case FPRecorderType.AnimationClip:
                        settings.AddRecorderSettings(blankAnimClipRecorder);
                        break;
                    case FPRecorderType.Movie:
                        switch (data.OutputFormatData.EncoderType)
                        {
                            case FPEncoderType.UnityEncoder:
                                var unityEncoder = data.OutputFormatData as MovieRecorderUnityMedia;
                                movieImageRecorder.EncoderSettings = unityEncoder.CoreEncoderSettings;
                                break;
                            case FPEncoderType.ProResEncoder:
                                var proResEncoder = data.OutputFormatData as MovieRecorderProRes;
                                movieImageRecorder.EncoderSettings = proResEncoder.ProResEncoderSettings;
                                break;
                            case FPEncoderType.GifEncoder:
                                var gifEncoder = data.OutputFormatData as MovieRecorderGif;
                                movieImageRecorder.EncoderSettings = gifEncoder.GifEncoderSettings;
                                break;
                        }
                        settings.AddRecorderSettings(movieImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAnimClipRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAudioRecorder);
                        break;
                    case FPRecorderType.ImageSequence:
                        var img = data.OutputFormatData as ImageSeqRecorder;
                        blankImageRecorder.OutputFormat = img.MediaFileFormat;
                        blankImageRecorder.EXRCompression = img.Compression;
                        blankImageRecorder.JpegQuality = img.Quality;
                        settings.AddRecorderSettings(blankImageRecorder);
                        ///RemoveScriptableObjectMemoryGenerated(movieImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAnimClipRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAudioRecorder);
                        break;
                    case FPRecorderType.Audio:
                        settings.AddRecorderSettings(blankAudioRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAnimClipRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(movieImageRecorder);
                        break;
                }

                var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
                //someWindow.
                someWindow.SetRecorderControllerSettings(settings);
                //update the settings file
                TheRecorderSettingsJSON = JsonUtility.ToJson(settings);
                HaveRecorderSettings = true;
                //now spawn the gameobjects

            }
            else
            {
                Debug.LogError($"You need to select an 'FP_RecorderDataSO' type, not a {Selection.activeObject.ToString()}");
            }
        }
        #endregion
        #region REPLACING OLD MENU STUFF For EASY BUTTON
        //[MenuItem(CreateAndAdd360RecorderToRecorderSettings,priority = FP_UtilityData.ORDER_SUBMENU_LVL3+5)]
        static protected void CreateAndAdd360Recorder()
        {
            Generate360Configuration();
            AddARecorderToUnityRecorder();
        }
        /// <summary>
        /// Core Function to add a Selected Recorder to the Recorder Editor Tool via Unity
        /// </summary>
        //[MenuItem(AddAnySingleRecorderToRecorderSettings, priority = FP_UtilityData.ORDER_SUBMENU_LVL3+1)]
        static protected void AddARecorderToUnityRecorder()
        {
            var data = Selection.activeObject as FP_RecorderDataSO;
            if (settings == null)
            {
                //repopulate from a saved file?
                if(settings != null){
                    Undo.RecordObject(settings, "Modify Recorder Settings");
                }
                if (HaveRecorderSettings)
                {
                    //load the settings from the json
                    settings = JsonUtility.FromJson<RecorderControllerSettings>(TheRecorderSettingsJSON);
                    Debug.LogWarning($"Found that we might have some settings in the EditorPrefs, but we need to load them from the JSON");

                }
                else
                {
                    Debug.LogWarning($"Got here and didn't have a recorder settings file");
                    settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                }
            }
            if (data != null)
            {
                //use this selected asset
                // Create recorder settings based on data.RecorderType, etc.
                //get current settings


                //get the current recordcontrollersettings
                
                //var localSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                //var currentController = UnityEditor.Recorder.RecorderEditor
                settings.ExitPlayMode = true;
                settings.CapFrameRate = true;
                settings.FrameRate = 30;
                settings.FrameRatePlayback = FrameRatePlayback.Constant;


                //figure out what recorder to add based on the FP_RecorderDataSO
                var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                var movieImageRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
                var blankAudioRecorder = ScriptableObject.CreateInstance<AudioRecorderSettings>();
                var blankAnimClipRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
                //Input Settings first
                if(data.RecorderType == FPRecorderType.ImageSequence || data.RecorderType == FPRecorderType.Movie)
                {
                    blankImageRecorder.imageInputSettings = MovieImageSequenceInputTarget(data);
                    movieImageRecorder.ImageInputSettings = MovieImageSequenceInputTarget(data);
                    blankImageRecorder.OutputFile = data.OutputCameraData.FileName;
                    movieImageRecorder.OutputFile = data.OutputCameraData.FileName;
                    blankImageRecorder.name = data.RecorderName;
                    movieImageRecorder.name = data.RecorderName;
                }
                else
                {
                    if (data.RecorderType == FPRecorderType.Audio)
                    {
                        var castAudio = data.InputFormatData as AudioRecorder;
                        blankAudioRecorder = castAudio.ReturnUnityOutputFormat();
                        blankAudioRecorder.OutputFile = data.OutputCameraData.FileName;
                        blankAudioRecorder.name = data.RecorderName;
                    }
                    else
                    {
                        if (data.RecorderType == FPRecorderType.AnimationClip)
                        {
                            var castClip = data.InputFormatData as AnimationClipRecorder;
                            blankAnimClipRecorder = castClip.ReturnUnityOutputFormat(data.AnimationClipGameObject);
                            blankAnimClipRecorder.OutputFile = data.OutputCameraData.FileName;
                            blankAnimClipRecorder.name = data.RecorderName;
                        }
                    }
                }
                
                //output format now
                switch (data.RecorderType)
                {
                    case FPRecorderType.AnimationClip:
                        settings.AddRecorderSettings(blankAnimClipRecorder);
                        
                        //RemoveScriptableObjectMemoryGenerated(movieImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAudioRecorder);
                        break;
                    case FPRecorderType.Movie:
                        switch (data.OutputFormatData.EncoderType)
                        {
                            case FPEncoderType.UnityEncoder:
                                var unityEncoder = data.OutputFormatData as MovieRecorderUnityMedia;
                                movieImageRecorder.EncoderSettings = unityEncoder.CoreEncoderSettings;
                                break;
                            case FPEncoderType.ProResEncoder:
                                var proResEncoder = data.OutputFormatData as MovieRecorderProRes;
                                movieImageRecorder.EncoderSettings = proResEncoder.ProResEncoderSettings;
                                break;
                            case FPEncoderType.GifEncoder:
                                var gifEncoder = data.OutputFormatData as MovieRecorderGif;
                                movieImageRecorder.EncoderSettings = gifEncoder.GifEncoderSettings;
                                break;
                        }
                        settings.AddRecorderSettings(movieImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAnimClipRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAudioRecorder);
                        break;
                    case FPRecorderType.ImageSequence:
                        var img = data.OutputFormatData as ImageSeqRecorder;
                        blankImageRecorder.OutputFormat = img.MediaFileFormat;
                        blankImageRecorder.EXRCompression = img.Compression;
                        blankImageRecorder.JpegQuality = img.Quality;
                        settings.AddRecorderSettings(blankImageRecorder);
                        ///RemoveScriptableObjectMemoryGenerated(movieImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAnimClipRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAudioRecorder);
                        break;
                    case FPRecorderType.Audio:
                        settings.AddRecorderSettings(blankAudioRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankAnimClipRecorder);
                        //RemoveScriptableObjectMemoryGenerated(blankImageRecorder);
                        //RemoveScriptableObjectMemoryGenerated(movieImageRecorder);
                        break;
                }

                var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
                //someWindow.
                someWindow.SetRecorderControllerSettings(settings);
                //update the settings file
                TheRecorderSettingsJSON = JsonUtility.ToJson(settings);
                HaveRecorderSettings = true;
                CacheRecorderControllerSettings();
                //EditorUtility.SetDirty(controller.Settings);
                //AssetDatabase.SaveAssetIfDirty(controller.Settings);
            }
            else
            {
                Debug.LogError($"You need to select an 'FP_RecorderDataSO' type, not a {Selection.activeObject.ToString()}");
            }
        }
        #endregion
        /// <summary>
        /// Clean up items we might have created on the editor side
        /// </summary>
        /// <param name="myScriptableObject"></param>
        /// <returns></returns>
        private static bool RemoveScriptableObjectMemoryGenerated(UnityEngine.Object myScriptableObject)
        {
            string assetPath = AssetDatabase.GetAssetPath(myScriptableObject);
            bool result = AssetDatabase.DeleteAsset(assetPath);

            if (result)
            {
                Debug.Log("Asset deleted successfully.");
            }
            else
            {
                Debug.Log("Asset deletion failed.");
            }
            return result;
        }
        private static ImageInputSettings MovieImageSequenceInputTarget(FP_RecorderDataSO data)
        {
            switch (data.InputFormatData.Source)
            {
                case FPInputSettings.GameView:
                    var dataInputGameViewCast = data.InputFormatData as GameViewCamRecordData;
                    return dataInputGameViewCast.GameViewSettings;
                    
                case FPInputSettings.TargetedCamera:
                    var dataInputTargetCast = data.InputFormatData as TargetedCamRecordData;
                    return dataInputTargetCast.TargetedCameraSettings;
                    
                case FPInputSettings.a360View:
                    var dataInput360Cast = data.InputFormatData as ThreeSixtyRecordData;
                    return dataInput360Cast.ThreeSixtyCameraSettings;
                    
                case FPInputSettings.RenderTextureAsset:
                    var dataInputTextureCast = data.InputFormatData as FPRenderTextureRecordData;
                    return dataInputTextureCast.RenderTextureCameraSettings;
                    
                case FPInputSettings.TextureSampling:
                    var dataInputTextureSamplingCast = data.InputFormatData as FPTextureSamplingRecordData;
                    return dataInputTextureSamplingCast.RenderTextureSamplingSettings;
                default:
                    return null;
            }
        }
        /// <summary>
        /// Create Asset at path
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="assetPath"></param>
        static protected void CreateAssetAt(UnityEngine.Object asset, string assetPath)
        {
            // Create the asset
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            // Focus the asset in the Unity Editor
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(asset, "Create " + asset.name);
            // Optionally, log the creation
            Debug.Log("ExampleAsset created at " + assetPath);
        }
        public string ReturnProductName()
        {
            return FP_RecorderUtility.PRODUCT_NAME;
        }

        public string ReturnSamplePath()
        {
            return FP_RecorderUtility.ReturnSamplesPath();
        }
    }

}
