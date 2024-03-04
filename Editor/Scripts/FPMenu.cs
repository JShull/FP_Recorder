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

namespace FuzzPhyte.Recorder.Editor
{
    public class FPMenu : MonoBehaviour, IFPProductEditorUtility
    {
        #region Editor Menu Configuration
        private const string SettingName = "FP_RecorderSettings";
        private const string NumberCamTags = "FP_CamTagsCount";
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
        //Configuration
        private const string DefaultConfigurationFileMenu = CustomMenuBasePath + "/Create " + FP_RecorderUtility.CAT0;
        private const string CGenericConfigMenu = DefaultConfigurationFileMenu + "/Default Config.";
        private const string C360ConfigMenu = DefaultConfigurationFileMenu + "/360 Config.";
        //Input Format
        private const string CamThreeSixtyInput = InputFile + "/360Cam";

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
            GenerateRecorder(FPRecorderType.Movie, dataPath.Item2, "AMovieRecorder.asset");
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
            GenerateRecorder(FPRecorderType.AnimationClip, dataPath.Item2, "AAnimClipRecorder.asset");
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
            GenerateRecorder(FPRecorderType.ImageSequence, dataPath.Item2, "ImageSequenceRecorder.asset");
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
            GenerateRecorder(FPRecorderType.Audio, dataPath.Item2, "AudioRecorder.asset");
        }
        [MenuItem(AudioRecorderMenu, true)]
        static protected bool CreateAudioRecorder()
        {
            return RecorderEnabled;
        }
        static protected void GenerateRecorder(FPRecorderType theType, string fulllocalPath, string assetName)
        {
            // The asset to be created
            var asset = FP_RecorderDataSO.CreateInstance(theType);
            
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
            var asset = ImageSeqRecorder.CreateInstance(FPMediaFileFormat.PNG, FPCompressionTypes.Zip, 100);
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
        [MenuItem("FuzzPhyte/FP_Recorder/Create Output File/<Recorder><Take> File",priority =FP_UtilityData.ORDER_SUBMENU_LVL2)]
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

            var imgSeqAsset = ImageSeqRecorder.CreateInstance(FPMediaFileFormat.PNG, FPCompressionTypes.Zip, 100);
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
