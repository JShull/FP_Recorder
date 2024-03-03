using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder.Input;
using FuzzPhyte.Utility;
using FuzzPhyte.Utility.Editor;
using UnityEngine.WSA;
using UnityEditor.VersionControl;

namespace FuzzPhyte.Recorder.Editor
{
    public class FPMenu : MonoBehaviour
    {
        #region Recorder Generation
        [MenuItem("FuzzPhyte/FP_Recorder/CreateRecorder/Movie", priority = FP_UtilityData.ORDER_SUBMENU_LVL1)]
        static protected void GenerateMovieRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.Movie, dataPath.Item2, "AMovieRecorder.asset");
        }
        [MenuItem("FuzzPhyte/FP_Recorder/CreateRecorder/AnimClip", priority = FP_UtilityData.ORDER_SUBMENU_LVL1+1)]
        static protected void GenerateAnimClipRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.AnimationClip, dataPath.Item2, "AAnimClipRecorder.asset");
        }
        [MenuItem("FuzzPhyte/FP_Recorder/CreateRecorder/ImageSequence", priority = FP_UtilityData.ORDER_SUBMENU_LVL1 + 2)]
        static protected void GenerateImageSeqRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.ImageSequence, dataPath.Item2, "ImageSequenceRecorder.asset");
        }
        [MenuItem("FuzzPhyte/FP_Recorder/CreateRecorder/Audio", priority = FP_UtilityData.ORDER_SUBMENU_LVL1 + 3)]
        static protected void GenerateAudioRecorder()
        {
            // Define the folder path where the asset will be saved            
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT1);
            GenerateRecorder(FPRecorderType.Audio, dataPath.Item2, "AudioRecorder.asset");
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
        [MenuItem("FuzzPhyte/FP_Recorder/CreateOutputFormat/AnimationClip", priority = FP_UtilityData.ORDER_SUBMENU_LVL2)]
        static protected void GenerateOutputAnimationClip()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);

            var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatAnimationClip.asset");

            //create the asset
            CreateAssetAt(asset, assetPath);

        }
        [MenuItem("FuzzPhyte/FP_Recorder/CreateOutputFormat/MovieEncoderUnity", priority = FP_UtilityData.ORDER_SUBMENU_LVL2+1)]
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
        [MenuItem("FuzzPhyte/FP_Recorder/CreateOutputFormat/MovieEncoderProRes",priority = FP_UtilityData.ORDER_SUBMENU_LVL2+2)]
        static protected void GenerateOutputMovieEncoderProRes()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            
            var ProResEncoderSettings = new UnityEditor.Recorder.Encoder.ProResEncoderSettings();
            ProResEncoderSettings.Format = UnityEditor.Recorder.Encoder.ProResEncoderSettings.OutputFormat.ProRes4444XQ;
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatEncoderProRes.asset");
            var asset = MovieRecorderProRes.CreateInstance(true, ProResEncoderSettings);
            
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem("FuzzPhyte/FP_Recorder/CreateOutputFormat/MovieEncoderGif",priority =FP_UtilityData.ORDER_SUBMENU_LVL2+3)]
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
        [MenuItem("FuzzPhyte/FP_Recorder/CreateOutputFormat/ImageSequence",priority = FP_UtilityData.ORDER_SUBMENU_LVL2+4)]
        static protected void GenerateOutputImageSequence()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatImgSequence.asset");
            var asset = ImageSeqRecorder.CreateInstance(FPMediaFileFormat.PNG, FPCompressionTypes.Zip, 100);
            CreateAssetAt(asset, assetPath);
        }
        [MenuItem("FuzzPhyte/FP_Recorder/CreateOutputFormat/Audio",priority = FP_UtilityData.ORDER_SUBMENU_LVL2+5)]
        static protected void GenerateOutputAudio()
        {
            var dataPath = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT3);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + "/OutputFormatAudio.asset");
            var asset = AudioRecorder.CreateInstance();
            CreateAssetAt(asset, assetPath);
        }
        static protected void GenerateOutput()
        {
           
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

            // Optionally, log the creation
            Debug.Log("ExampleAsset created at " + assetPath);
        }


        [MenuItem("Window/FuzzPhyte/FP_Recorder/CreateConfiguration",priority = FP_UtilityData.ORDER_SUBMENU_LVL1+1)]
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

        
        
    }
    
}
