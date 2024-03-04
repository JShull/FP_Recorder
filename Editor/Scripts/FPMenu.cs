using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder.Input;
using FuzzPhyte.Utility;
using FuzzPhyte.Utility.Editor;
using UnityEngine.WSA;
using UnityEditor.VersionControl;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;
using UnityEditor.Recorder;
using NUnit.Framework;

namespace FuzzPhyte.Recorder.Editor
{
    public class FPMenu : MonoBehaviour, IFPProductEditorUtility
    {
        private static RecorderControllerSettings settings;// = new RecorderControllerSettings();
        #region Editor Menu Configuration
        private const string SettingName = "FP_RecorderSettings";
        private const string NumberCamTags = "FP_CamTagsCount";
        private const string JSONEditorName = "FP_RecorderSettingsJSON";
        private const string HaveSettingsFileName = "FP_HaveSettings";
        //
        private const string CustomMenuBasePath = FP_UtilityData.MENU_COMPANY+"/"+FP_RecorderUtility.PRODUCT_NAME;
        private const string SetupMenuBase = FP_UtilityData.MENU_COMPANY + "/" + FP_RecorderUtility.PRODUCT_NAME + "/Setup";
        private const string SetupAddTenCameras = SetupMenuBase + "/Generate Ten CameraTags";
        private const string SetupAddFiveCameras = SetupMenuBase + "/Generate Five CameraTags";
        private const string SetupRemoveTenCameras = SetupMenuBase + "/Remove Ten CameraTags";
        private const string SetupRemoveFiveCameras = SetupMenuBase + "/Remove Five CameraTags";
        private const string RemoveAllFPCameras = SetupMenuBase + "/Reset All FPCameraTags";
        private const string OutputFormat = CustomMenuBasePath + "/Create " + FP_RecorderUtility.CAT3;
        private const string InputFile = CustomMenuBasePath + "/Create " + FP_RecorderUtility.CAT2;
        private const string RecorderType = CustomMenuBasePath + "/Create " + FP_RecorderUtility.CAT1;
        private const string OutputFile = CustomMenuBasePath + "/Create " + FP_RecorderUtility.CAT4;
        //Configuration
        private const string DefaultConfigurationFileMenu = CustomMenuBasePath + "/Create " + FP_RecorderUtility.CAT0;
        private const string CGenericConfigMenu = DefaultConfigurationFileMenu + "/Default Config.";
        private const string C360ConfigMenu = DefaultConfigurationFileMenu + "/360 Config.";
        //Add Recorder
        private const string AddRecorderFileMenu = CustomMenuBasePath + "/Create & Place " + FP_RecorderUtility.CAT5;
        private const string AddAnySingleRecorderToRecorderSettings = AddRecorderFileMenu + "/Add Selected Recorder";
        private const string CreateAndAdd360RecorderToRecorderSettings = AddRecorderFileMenu + "/Create & Add: 360";
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
            get { return EditorPrefs.GetInt(NumberCamTags); }
            set { EditorPrefs.SetInt(NumberCamTags, value); }
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
        #endregion
        #region Tags & Setup

        [MenuItem(SetupAddTenCameras,priority = FP_UtilityData.ORDER_SUBMENU_LVL5)]
        static protected void AddTenCameraTags()
        {
            AddNumberTags(10);
        }
        [MenuItem(SetupAddFiveCameras, priority = FP_UtilityData.ORDER_SUBMENU_LVL5+1)]
        static protected void AddFiveCameraTags()
        {
            AddNumberTags(5);
        }
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
        //runs once when the editor is loaded and we check to see if it already exists in JSON
        //called from another script
        public static void SetupSettingsFileOnBoot()
        {
            if (HaveRecorderSettings)
            {
                Debug.Log($"String message that should be maybe JSON BEGINS HERE| {TheRecorderSettingsJSON}");
                settings = JsonUtility.FromJson<RecorderControllerSettings>(TheRecorderSettingsJSON);
                //we have something in the editorprefs, but we need to load it from the JSON
                Debug.LogWarning($"Found that we might have some settings in the EditorPrefs, but we need to load them from the JSON");
            }
            else
            {
                Debug.LogWarning($"no Settings file in the editorPrefs");
                settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                settings.ExitPlayMode = true;
                settings.CapFrameRate = true;
                settings.FrameRate = 30;
                settings.FrameRatePlayback = FrameRatePlayback.Constant;
                //save the settings as json and store them to my editerprefs
                TheRecorderSettingsJSON = JsonUtility.ToJson(settings);
                HaveRecorderSettings = true;
            }
            
        }
        /// <summary>
        /// Adds 'x' number of camera tags
        /// </summary>
        /// <param name="numberTags"></param>
        static protected void AddNumberTags(int numberTags)
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

            var someWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
            
            
            Menu.SetChecked("FuzzPhyte/FP_Recorder/Ready", true);
            EditorUtility.DisplayDialog($"Added {numberTags} Camera Tags", $"'{FP_RecorderUtility.CamTAG}' with an '0,1,2,...' have been adedd! You now have {NumberCameraTags} FP_Record camera tags in the Project.", "OK");

        }
        
        [MenuItem(SetupRemoveFiveCameras,priority =FP_UtilityData.ORDER_SUBMENU_LVL5+3)]
        static protected void RemoveFiveCameraTags()
        {
            RemoveNumberTags(5);
        }
        [MenuItem(SetupRemoveTenCameras,priority =FP_UtilityData.ORDER_SUBMENU_LVL5+2)]
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
        [MenuItem("FuzzPhyte/FP_Recorder/Ready",priority=FP_UtilityData.ORDER_SUBMENU_LVL4)]
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

        [MenuItem(MovieRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL2)]
        static protected void GenerateMovieRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.Movie, dataPath.Item2, "AMovieRecorder.asset", "FP_Movie");
        }
        [MenuItem(MovieRecorderMenu, true)]
        static bool CreateRecorderMovieActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(AnimClipRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL2+1)]
        static protected void GenerateAnimClipRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.AnimationClip, dataPath.Item2, "AAnimClipRecorder.asset","FP_AnimClip");
        }
        [MenuItem(AnimClipRecorderMenu, true)]
        static bool CreateRecorderAnimClipActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(ImageSeqRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL2 + 2)]
        static protected void GenerateImageSeqRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.ImageSequence, dataPath.Item2, "ImageSequenceRecorder.asset","FP_ImgSequence");
        }
        [MenuItem(ImageSeqRecorderMenu, true)]
        static bool CreateRecorderImageSeqActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(AudioRecorderMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL2 + 3)]
        static protected void GenerateAudioRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.Audio, dataPath.Item2, "AudioRecorder.asset","FP_Audio");
        }
        [MenuItem(AudioRecorderMenu, true)]
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

        #region Recorders OutputFormat
        [MenuItem(AnimClipOutput, priority = FP_UtilityData.ORDER_SUBMENU_LVL2)]
        static protected void GenerateOutputAnimationClip()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);

            var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatAnimationClip.asset");

            //create the asset
            CreateAssetAt(asset, assetPath);

        }
        [MenuItem(AnimClipOutput, true)]
        static bool OutputAnimationClipActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(MovieOutputMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL2+1)]
        static protected void GenerateOutputMovieEncoderUnity()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            var CoreEncoderSettings = new UnityEditor.Recorder.Encoder.CoreEncoderSettings();
            CoreEncoderSettings.Codec = UnityEditor.Recorder.Encoder.CoreEncoderSettings.OutputCodec.MP4;
            CoreEncoderSettings.EncodingQuality = UnityEditor.Recorder.Encoder.CoreEncoderSettings.VideoEncodingQuality.High;
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderUnity.asset");
            var asset = MovieRecorderUnityMedia.CreateInstance(true, CoreEncoderSettings);
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(MovieOutputMenu, true)]
        static bool OutputMovieEncoderUnityActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(MovieOutputProResMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL2+2)]
        static protected void GenerateOutputMovieEncoderProRes()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            
            var ProResEncoderSettings = new UnityEditor.Recorder.Encoder.ProResEncoderSettings();
            ProResEncoderSettings.Format = UnityEditor.Recorder.Encoder.ProResEncoderSettings.OutputFormat.ProRes4444XQ;
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderProRes.asset");
            var asset = MovieRecorderProRes.CreateInstance(true, ProResEncoderSettings);
            
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(MovieOutputProResMenu, true)]
        static bool OutputMovieEncoderProResActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(MovieOutputGif, priority =FP_UtilityData.ORDER_SUBMENU_LVL2+3)]
        static protected void GenerateOutputMovieEncoderGif()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            var gifEncoder = new UnityEditor.Recorder.Encoder.GifEncoderSettings();
            gifEncoder.Loop = true;
            gifEncoder.Quality = 90;
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderGif.asset");
            var asset = MovieRecorderGif.CreateInstance(gifEncoder);
            CreateAssetAt(asset, assetPath);

        }
        [MenuItem(MovieOutputGif, true)]
        static bool OutputMovieEncoderGifActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(ImageSeqOutput, priority = FP_UtilityData.ORDER_SUBMENU_LVL2+4)]
        static protected void GenerateOutputImageSequence()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatImgSequence.asset");
            var asset = ImageSeqRecorder.CreateInstance( ImageRecorderSettings.ImageRecorderOutputFormat.PNG , ImageRecorderSettings.EXRCompressionType.Zip, 100);
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(ImageSeqOutput, true)]
        static bool OutputMovieEncoderImageSequenceActive()
        {
            return RecorderEnabled;
        }
        [MenuItem(AudioOutput, priority = FP_UtilityData.ORDER_SUBMENU_LVL2+5)]
        static protected void GenerateOutputAudio()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatAudio.asset");
            var asset = AudioRecorder.CreateInstance();
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(AudioOutput, true)]
        static bool OutputMovieAudioActive()
        {
            return RecorderEnabled;
        }
        #endregion

        #region Recorder InputFormat
        [MenuItem(CamThreeSixtyInput, priority = FP_UtilityData.ORDER_SUBMENU_LVL2)]
        static protected void GenerateInputFileCamThreeSixty()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT2);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/InputFileCam360.asset");

            var asset = GenerateCamThreeSixty();
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(CamThreeSixtyInput, true)]
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
        [MenuItem(RecordTakeOutputFile, priority =FP_UtilityData.ORDER_SUBMENU_LVL2)]
        static protected void GenerateOutputFileRecorderTake()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT4);
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFileRecordTake.asset");
            List<FPWildCards> _cards = new List<FPWildCards>
            {
                FPWildCards.RECORDER,
                FPWildCards.TAKE
            };
            var asset = FP_OutputFileSO.CreateInstance(_cards, UnityEditor.Recorder.OutputPath.Root.AssetsFolder);
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem(CGenericConfigMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL3+1)]
        static protected void GenerateRecorderConfiguration()
        {
            // Define the folder path where the asset will be saved
            string folderPath = FP_RecorderUtility.SAMPLESPATH + "/" + FP_RecorderUtility.CAT1;

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
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

        [MenuItem(C360ConfigMenu, priority = FP_UtilityData.ORDER_SUBMENU_LVL3)]
        static protected void Generate360Configuration()
        {
            // Define the folder path where the asset will be saved
            //need to generate all of the supporting files first

            string folderPath = FP_RecorderUtility.SAMPLESPATH + "/" + FP_RecorderUtility.CAT1;

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            }

            // The asset to be created
            var recorderAsset = ScriptableObject.CreateInstance<FP_RecorderDataSO>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/A360Configuration.asset");
            recorderAsset.RecorderType = FPRecorderType.ImageSequence;
            recorderAsset.RecorderName = "FP_Generated_ImgSeq";
            //Create all of the other files we need for 360
            //ThreeSixtyRecordData:FP_InputDataSO
            var inputAsset = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT2);
            string inputAssetPath = AssetDatabase.GenerateUniqueAssetPath(inputAsset.Item2 + "/InputFileCam360.asset");
            //outputPath File
            var outputFile = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT4);
            var outputFileAssetPath = AssetDatabase.GenerateUniqueAssetPath(outputFile.Item2 + "/OutputFileRecordTake.asset");
            List<FPWildCards> _cards = new List<FPWildCards>
            {
                FPWildCards.RECORDER,
                FPWildCards.TAKE
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
            var outputAsset = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
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
        [MenuItem(CreateAndAdd360RecorderToRecorderSettings,priority = FP_UtilityData.ORDER_SUBMENU_LVL3)]
        static protected void CreateAndAdd360Recorder()
        {
            Generate360Configuration();
            AddARecorderToUnityRecorder();
        }
        /// <summary>
        /// Core Function to add a Selected Recorder to the Recorder Editor Tool via Unity
        /// </summary>
        [MenuItem(AddAnySingleRecorderToRecorderSettings, priority = FP_UtilityData.ORDER_SUBMENU_LVL3+1)]
        static protected void AddARecorderToUnityRecorder()
        {
            var data = Selection.activeObject as FP_RecorderDataSO;
            if (settings == null)
            {
                //repopulate from a saved file?
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
                //EditorUtility.SetDirty(controller.Settings);
                //AssetDatabase.SaveAssetIfDirty(controller.Settings);
            }
            else
            {
                Debug.LogError($"You need to select an 'FP_RecorderDataSO' type, not a {Selection.activeObject.ToString()}");
            }
        }
        /// <summary>
        /// Clean up items we might have created on the editor side
        /// </summary>
        /// <param name="myScriptableObject"></param>
        /// <returns></returns>
        private static bool RemoveScriptableObjectMemoryGenerated(Object myScriptableObject)
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
        static protected void CreateAssetAt(Object asset, string assetPath)
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
            return FP_RecorderUtility.SAMPLESPATH;
        }
    }

}
