using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Recorder;
using UnityEditor.Presets;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder.Encoder;
using FuzzPhyte.Utility.Editor;
using UnityEditor;

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
        public List<RecorderSettings> ReturnUnityRecorderByData()
        {
            //take our RecorderData and dump out the RecorderSettings by Data Type
            var RecList = new List<RecorderSettings>();

            for(int i = 0; i < RecorderData.Count; i++)
            {
                var recData = RecorderData[i];
                var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                var movieImageRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
                var blankAudioRecorder = ScriptableObject.CreateInstance<AudioRecorderSettings>();
                var blankAnimClipRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
                //parse through it
                if (recData.RecorderType == FPRecorderType.ImageSequence || recData.RecorderType == FPRecorderType.Movie)
                {
                    //image input settings from data by recData Source
                    switch (recData.Source)
                    {
                        case FPInputSettings.GameView:
                            blankImageRecorder.imageInputSettings = recData.GameViewCamInputFormatData;
                            movieImageRecorder.ImageInputSettings = recData.GameViewCamInputFormatData;
                            break;
                        case FPInputSettings.TargetedCamera:
                            blankImageRecorder.imageInputSettings = recData.TargetCamInputFormatData;
                            movieImageRecorder.ImageInputSettings = recData.TargetCamInputFormatData;
                            break;
                        case FPInputSettings.a360View:
                            blankImageRecorder.imageInputSettings = recData.ThreeSixInputFormatData;
                            movieImageRecorder.ImageInputSettings = recData.ThreeSixInputFormatData;
                            break;
                        case FPInputSettings.RenderTextureAsset:
                            blankImageRecorder.imageInputSettings = recData.FPRenderTextureInputFormatData;
                            movieImageRecorder.ImageInputSettings = recData.FPRenderTextureInputFormatData;
                            break;
                        case FPInputSettings.TextureSampling:
                            blankImageRecorder.imageInputSettings = recData.FPTextureSamplingInputFormatData;
                            movieImageRecorder.ImageInputSettings = recData.FPTextureSamplingInputFormatData;
                            break;
                       
                    }
                    blankImageRecorder.OutputFile = recData.OutputFileData.FileName;
                    movieImageRecorder.OutputFile = recData.OutputFileData.FileName;
                    blankImageRecorder.name = recData.RecorderName;
                    movieImageRecorder.name = recData.RecorderName;

                    //need encoder types
                    if (recData.RecorderType == FPRecorderType.ImageSequence)
                    {
                        //might have to add in Image Compression information to the format
                        blankImageRecorder.OutputFormat = recData.ImageSeqOutputFormatData.OutputFormat;
                        blankImageRecorder.EXRCompression = recData.ImageSeqOutputFormatData.EXRCompression;
                        blankImageRecorder.JpegQuality = recData.ImageSeqOutputFormatData.JpegQuality;
                        RecList.Add(blankImageRecorder);
                    }
                    else
                    {
                        //must be a movie
                        //encoder options
                        
                        switch (recData.Encoder)
                        {
                            case FPEncoderType.UnityEncoder:
                                movieImageRecorder.EncoderSettings = recData.MovieUnityMediaOutputFormatData;
                                break;
                            case FPEncoderType.ProResEncoder:
                                movieImageRecorder.EncoderSettings = recData.MovieProResOutputFormatData;
                                break;
                            case FPEncoderType.GifEncoder:
                                movieImageRecorder.EncoderSettings = recData.MovieGifOutputFormatData;
                                break;
                        }
                        RecList.Add(movieImageRecorder);
                    }
                }
                else
                {
                    if (recData.RecorderType == FPRecorderType.Audio)
                    {
                        blankAudioRecorder = recData.AudioInputFormatData;
                        blankAudioRecorder.OutputFile = recData.OutputFileData.FileName;
                        blankAudioRecorder.name = recData.RecorderName;
                        RecList.Add(blankAudioRecorder);
                    }
                    else
                    {
                        if(recData.RecorderType == FPRecorderType.AnimationClip)
                        {
                            blankAnimClipRecorder = recData.AnimationClipInputFormatData;
                            blankAnimClipRecorder.OutputFile = recData.OutputFileData.FileName;
                            blankAnimClipRecorder.name = recData.RecorderName;
                            RecList.Add(blankAnimClipRecorder);
                        }
                    }
                }
                

            }

            return RecList;
        }
        #endregion

    }
    [Serializable]
    public struct FPRecorderDataStruct
    {

        public string RecorderName;
        public FPRecorderType RecorderType;
        public FPInputSettings Source;
        public FPEncoderType Encoder;
        //all possible inputs
        //Anim/Audio
        public AnimationRecorderSettings AnimationClipInputFormatData;
        public AudioRecorderSettings AudioInputFormatData;
        //Video/Image
        public Camera360InputSettings ThreeSixInputFormatData;
        public RenderTextureInputSettings FPRenderTextureInputFormatData;
        public RenderTextureSamplerSettings FPTextureSamplingInputFormatData;
        public GameViewInputSettings GameViewCamInputFormatData;
        public CameraInputSettings TargetCamInputFormatData;

        //public FP_InputDataSO InputFormatData;
        //all possible OutputFormats
        public ImageRecorderSettings ImageSeqOutputFormatData;
        public ProResEncoderSettings MovieProResOutputFormatData;
        public GifEncoderSettings MovieGifOutputFormatData;
        public CoreEncoderSettings MovieUnityMediaOutputFormatData;

        //public FP_OutputFormatSO OutputFormatData;
        //all possible outputfiles - thankfully only one type

        public FP_OutputFileSO OutputFileData;

        public int NumberOfCameras;
        public GameObject GameObjectRef;
        public void Init(FPRecorderDataStruct passedData)
        {
            this = passedData;
        }
        public static FPRecorderDataStruct CreateEmptyDataClassObject(FPRecorderType recType, FPInputSettings sourceT, FPEncoderType encoderT,string recordName, GameObject gameObjectSceneRef=null)
        {
            var data = new FPRecorderDataStruct();
            //lets setup our output data first
            data = SetupOutputFile(data);
            data.Source = sourceT;
            data.Encoder = encoderT;
            data.RecorderName = recordName;
            data.GameObjectRef = gameObjectSceneRef;
            //setup our data files by the recorder type
            switch (recType)
            {
                case FPRecorderType.AnimationClip:
                    var blankAnimClipRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
                    blankAnimClipRecorder = ReturnUnityOutputFormatAnimtion(gameObjectSceneRef, AnimationInputSettings.CurveSimplificationOptions.Lossy, true, true);
                    data.AnimationClipInputFormatData = blankAnimClipRecorder;
                    //special case
                    break;
                case FPRecorderType.Movie:
                    data = ReturnImageInputSettingsBySource(sourceT, data);
                    switch (encoderT)
                    {
                        case FPEncoderType.ImageEncoder:
                        case FPEncoderType.NoEncoder:
                            break;
                        case FPEncoderType.UnityEncoder:
                            data.MovieUnityMediaOutputFormatData = new UnityEditor.Recorder.Encoder.CoreEncoderSettings();
                            //data.MovieUnityMediaOutputFormatData.CoreEncoderSettings = new UnityEditor.Recorder.Encoder.CoreEncoderSettings();
                            break;
                        case FPEncoderType.ProResEncoder:
                            data.MovieProResOutputFormatData = new UnityEditor.Recorder.Encoder.ProResEncoderSettings();
                            //data.MovieProResOutputFormatData.ProResEncoderSettings = new UnityEditor.Recorder.Encoder.ProResEncoderSettings();
                            break;
                        case FPEncoderType.GifEncoder:
                            data.MovieGifOutputFormatData = new UnityEditor.Recorder.Encoder.GifEncoderSettings();
                            //data.MovieGifOutputFormatData.GifEncoderSettings = new UnityEditor.Recorder.Encoder.GifEncoderSettings();
                            break;
                    }
                    break;
                case FPRecorderType.ImageSequence:
                    var blankImageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                    data = ReturnImageInputSettingsBySource(sourceT,data);
                    data.ImageSeqOutputFormatData = blankImageRecorder;
                    break;
                case FPRecorderType.Audio:
                    var blankAudioRecorder = ScriptableObject.CreateInstance<AudioRecorderSettings>();
                    blankAudioRecorder = ReturnUnityOutputFormatAudio();
                    data.AudioInputFormatData = blankAudioRecorder;
                    //special case
                    break;
            }
            data.Init(data);
            return data;
        }

        private static FPRecorderDataStruct SetupOutputFile(FPRecorderDataStruct data)
        {
            //outputPath File
            var outputFile = FP_Utility_Editor.CreateAssetFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.CAT4);
            var outputFileAssetPath = AssetDatabase.GenerateUniqueAssetPath(outputFile.Item2 + "/OutputFileRecordTake.asset");
            List<FPWildCards> _cards = new List<FPWildCards>
            {
                FPWildCards.RECORDER,
                FPWildCards.TAKE
            };
            var outputFileAsset = FP_OutputFileSO.CreateInstance(_cards, UnityEditor.Recorder.OutputPath.Root.AssetsFolder);
            data.OutputFileData = outputFileAsset;
            return data;
        }

        //return Animation Recorder Settings
        private static AnimationRecorderSettings ReturnUnityOutputFormatAnimtion(GameObject animData,AnimationInputSettings.CurveSimplificationOptions animCompression, bool ClampedTangents, bool RecordedHierarchy)
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
        private static AudioRecorderSettings ReturnUnityOutputFormatAudio(GameObject gObject = null)
        {
            var otherFormatSettings = new UnityEditor.Recorder.AudioRecorderSettings();
            //otherFormatSettings.RecordMode
            //otherFormatSettings.fr
            return otherFormatSettings;
            //otherFormatSettings.InputsSettings
        }
        
        private static FPRecorderDataStruct ReturnImageInputSettingsBySource(FPInputSettings source, FPRecorderDataStruct dataObject)
        {
            //var imageInput = ScriptableObject.CreateInstance<ImageInputSettings>();
            //var imageInputSettings = new ImageInputSettings();
            switch (source)
            {
                case FPInputSettings.GameView:
                    //var someGameViewSettings = passedSettings as GameViewInputSettings;
                    dataObject.GameViewCamInputFormatData = new GameViewInputSettings();
                    break;
                    //return new GameViewInputSettings();
                    //return someGameViewSettings;
                case FPInputSettings.TargetedCamera:
                    dataObject.TargetCamInputFormatData = new CameraInputSettings();
                    break;
                    //return new CameraInputSettings();
                    //var someTargetedCamSettings = passedSettings as CameraInputSettings;
                    //return someTargetedCamSettings;
                case FPInputSettings.RenderTextureAsset:
                    dataObject.FPRenderTextureInputFormatData = new RenderTextureInputSettings();
                    break;
                    //return new RenderTextureInputSettings();
                    //var someRenderTextureSettings = passedSettings as RenderTextureInputSettings;
                    //return someRenderTextureSettings;
                case FPInputSettings.TextureSampling:
                    dataObject.FPTextureSamplingInputFormatData = new RenderTextureSamplerSettings();
                    break;
                    //return new RenderTextureSamplerSettings();
                //var someTextureSamplingSettings = passedSettings as RenderTextureSamplerSettings;
                //return someTextureSamplingSettings;
                case FPInputSettings.a360View:
                    dataObject.ThreeSixInputFormatData = new Camera360InputSettings();
                    break;
                    //return new Camera360InputSettings();
                default:
                     break;
            }
            return dataObject;
            
        }

    }

}
