using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Recorder;
using UnityEditor.Presets;
using UnityEditor.Recorder.Input;
namespace FuzzPhyte.Recorder.Editor
{
    [Serializable]
    public class FPRecorderSettingsJSON 
    {
        public FrameRatePlayback Playback;
        public RecordMode RecordMode;
        public int TargetFrame;
        public int StartIntervalFrame;
        public int EndIntervalFrame;
        public float StartTimeInterval;
        public float EndTimeIntervarl;
        public bool CapFPS = true;
        public List<FPRecorderDataStruct> RecorderData = new List<FPRecorderDataStruct>();
        //
        //generic input data class = FP_RecorderDataSO
        //
        /*
         *      
                var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                var movieImageRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
                var blankAudioRecorder = ScriptableObject.CreateInstance<AudioRecorderSettings>();
                var blankAnimClipRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
         * */

        #region Methods to Convert
        public RecorderControllerSettings ReturnUnityRecorderControllerSettingsFormat()
        {
            var MyPreset = new RecorderControllerSettings();
            switch (RecordMode)
            {
                case RecordMode.Manual:
                    MyPreset.SetRecordModeToManual();
                    break;
                case RecordMode.SingleFrame:
                    MyPreset.SetRecordModeToSingleFrame(TargetFrame);
                    break;
                case RecordMode.FrameInterval:
                    MyPreset.SetRecordModeToFrameInterval(StartIntervalFrame, EndIntervalFrame);
                    break;
                case RecordMode.TimeInterval:
                    MyPreset.SetRecordModeToTimeInterval(StartTimeInterval, EndTimeIntervarl);
                    break;
            }
            MyPreset.FrameRatePlayback = Playback;

            return MyPreset;

        }
        #endregion

    }
    [Serializable]
    public struct FPRecorderDataStruct
    {
        //public UnityEditor.Recorder.
        public string RecorderName;
        public FPRecorderType RecorderType;
        public FPInputSettings Source;
        //all possible inputs
        public Camera360InputSettings ThreeSixInputFormatData;
        public AnimationRecorderSettings AnimationClipInputFormatData;
        public AudioRecorderSettings AudioInputFormatData;
        public RenderTextureInputSettings FPRenderTextureInputFormatData;
        public RenderTextureSamplerSettings FPTextureSamplingInputFormatData;
        public GameViewInputSettings GameViewCamInputFormatData;
        public CameraInputSettings TargetCamInputFormatData;

        //public FP_InputDataSO InputFormatData;
        //all possible OutputFormats
        public ImageSeqRecorder ImageSeqOutputFormatData;
        public MovieRecorderProRes MovieProResOutputFormatData;
        public MovieRecorderGif MovieGifOutputFormatData;
        public MovieRecorderUnityMedia MovieUnityMediaOutputFormatData;

        //public FP_OutputFormatSO OutputFormatData;
        //all possible outputfiles - thankfully only one type

        public FP_OutputFileSO OutputCameraData;

        public int NumberOfCameras;
        public GameObject AnimationClipGameObject;
        public void Init(FPRecorderType recorderType, string name)
        {
            this.RecorderType = recorderType;
            this.RecorderName = name;
        }
        //return Animation Recorder Settings
        public AnimationRecorderSettings ReturnUnityOutputFormat(GameObject animData,AnimationInputSettings.CurveSimplificationOptions animCompression, bool ClampedTangents, bool RecordedHierarchy)
        {
            var newAnimRecordSettings = new AnimationRecorderSettings();
            var newInputSettings = new AnimationInputSettings();
            //pass settings
            newInputSettings.SimplyCurves = animCompression;
            newInputSettings.ClampedTangents = ClampedTangents;
            newInputSettings.Recursive = RecordedHierarchy;
            if (animData != null)
            {
                newInputSettings.gameObject = animData;
            }

            newAnimRecordSettings.AnimationInputSettings = newInputSettings;
            return newAnimRecordSettings;
        }
        //return Audio Recorder Settings
        public AudioRecorderSettings ReturnUnityOutputFormat(GameObject gObject = null)
        {
            var otherFormatSettings = new UnityEditor.Recorder.AudioRecorderSettings();
            //otherFormatSettings.RecordMode
            //otherFormatSettings.fr
            return otherFormatSettings;
            //otherFormatSettings.InputsSettings
        }
        public static FPRecorderDataStruct CreateDataInstance(FPRecorderType recType, FPInputSettings sourceType, string recordName)
        {
            var data = new FPRecorderDataStruct();
            
            //setup our data files by the recorder type
            switch (recType)
            {
                case FPRecorderType.AnimationClip:
                    var blankAnimClipRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
                    //special case
                    break;
                case FPRecorderType.Movie:
                    var movieImageRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
                    movieImageRecorder.ImageInputSettings = ReturnImageInputSettingsBySource(sourceType);
                    break;
                case FPRecorderType.ImageSequence:
                    var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                    blankImageRecorder.imageInputSettings = ReturnImageInputSettingsBySource(sourceType);
                    break;
                case FPRecorderType.Audio:
                    var blankAudioRecorder = ScriptableObject.CreateInstance<AudioRecorderSettings>();
                    //special case
                    break;
            }
            data.Init(recType, recordName);
            return data;
        }

        private static ImageInputSettings ReturnImageInputSettingsBySource(FPInputSettings source)
        {
            //var imageInput = ScriptableObject.CreateInstance<ImageInputSettings>();
            //var imageInputSettings = new ImageInputSettings();
            switch (source)
            {
                case FPInputSettings.GameView:
                    //var someGameViewSettings = passedSettings as GameViewInputSettings;
                    return new GameViewInputSettings();
                    //return someGameViewSettings;
                case FPInputSettings.TargetedCamera:
                    return new CameraInputSettings();
                    //var someTargetedCamSettings = passedSettings as CameraInputSettings;
                    //return someTargetedCamSettings;
                case FPInputSettings.RenderTextureAsset:
                    return new RenderTextureInputSettings();
                    //var someRenderTextureSettings = passedSettings as RenderTextureInputSettings;
                    //return someRenderTextureSettings;
                case FPInputSettings.TextureSampling:
                    return new RenderTextureSamplerSettings();
                    //var someTextureSamplingSettings = passedSettings as RenderTextureSamplerSettings;
                    //return someTextureSamplingSettings;
                default:
                    return null;
            }
            
        }

    }

}
