using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Recorder;
using UnityEditor.Presets;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder.Encoder;
using FuzzPhyte.Utility.Editor;
using FuzzPhyte.Utility;
using UnityEditor;
using UnityEngine.TestTools;
using Unity.Mathematics;
using System.Linq;
namespace FuzzPhyte.Recorder.Editor
{
    [Serializable]
    public class FPRecorderSettingsJSON 
    {
        public FrameRatePlayback Playback;
        public RecordMode RecordMode;
        public float FrameRate = 30;
        public int TargetFrame;
        public int StartIntervalFrame;
        public int EndIntervalFrame;
        public float StartTimeInterval;
        public float EndTimeInterval;
        public bool CapFPS = true;
        public bool ExitPlayMode = true;
        public string RecorderNotes;
        public List<FPRecorderDataStruct> RecorderData = new List<FPRecorderDataStruct>();
        public List<FPTransformStruct> CameraPlacements = new List<FPTransformStruct>();
        /// <summary>
        /// Frame Interval Mode Setup
        /// </summary>
        /// <param name="recMode"></param>
        /// <param name="targetFrame"></param>
        /// <param name="startIntervalFrame"></param>
        /// <param name="endIntervalFrame"></param>
        /// <param name="capFPS"></param>
        public FPRecorderSettingsJSON(RecordMode recMode,int targetFrame,int startIntervalFrame,int endIntervalFrame, float fps, FrameRatePlayback playback, bool capFPS,bool exitPlayMode=true)
        {
            RecordMode = recMode;
            Playback = playback;
            if(RecordMode != RecordMode.FrameInterval)
            {
                //we passed frame information but aren't using frame interval
                Debug.LogWarning($"You passed Frame Interval Data but have the {RecordMode} mode on, this is usually a mixup and you might have wanted a different constructor");
                CapFPS = capFPS;
                ExitPlayMode = exitPlayMode;
                if (RecordMode == RecordMode.SingleFrame)
                {
                    TargetFrame = 1;
                }
                if (RecordMode == RecordMode.TimeInterval)
                {
                    StartTimeInterval = 0;
                    EndTimeInterval = 1;
                }
                return;
            }
            FrameRate = fps;
            ExitPlayMode = exitPlayMode;
            TargetFrame = targetFrame;
            StartIntervalFrame = startIntervalFrame;
            EndIntervalFrame = endIntervalFrame;
            CapFPS = capFPS;
        }
        /// <summary>
        /// Time Interval Mode Setup
        /// </summary>
        /// <param name="recMode"></param>
        /// <param name="targetFrame"></param>
        /// <param name="startTimeInterval"></param>
        /// <param name="endTimeInterval"></param>
        /// <param name="capFPS"></param>
        public FPRecorderSettingsJSON(RecordMode recMode,int targetFrame,float startTimeInterval, float endTimeInterval, float fps, FrameRatePlayback playback,bool capFPS,bool exitPlayMode=true)
        {
            RecordMode = recMode;
            Playback = playback;
            if (RecordMode != RecordMode.TimeInterval)
            {
                //we passed frame information but aren't using frame interval
                Debug.LogWarning($"You passed Time Interval Data but have the {RecordMode} mode on, this is usually a mixup and you might have wanted a different constructor");
                CapFPS = capFPS;
                ExitPlayMode = exitPlayMode;
                if (RecordMode == RecordMode.FrameInterval)
                {
                    StartIntervalFrame = 0;
                    EndIntervalFrame = 1;

                }
                if (RecordMode == RecordMode.SingleFrame)
                {
                    TargetFrame = 1;
                }
                return;
            }
            FrameRate = fps;
            ExitPlayMode = exitPlayMode;
            TargetFrame = targetFrame;
            StartTimeInterval = startTimeInterval;
            EndTimeInterval = endTimeInterval;
            CapFPS = capFPS;
        }
        public FPRecorderSettingsJSON(RecordMode recMode, int singleFrame,bool capFPS, bool exitPlayMode=true)
        {
            RecordMode = recMode;
            Playback = FrameRatePlayback.Constant;
            if (RecordMode != RecordMode.SingleFrame)
            {
                Debug.LogWarning($"You passed Single Frame Data but have the {RecordMode} mode active, this is usually a mixup and you might have wanted a different constructor");
                CapFPS = capFPS;
                ExitPlayMode = exitPlayMode;
                if (RecordMode == RecordMode.FrameInterval)
                {
                    StartIntervalFrame = 0;
                    EndIntervalFrame = 1;
                    
                }
                if(RecordMode == RecordMode.TimeInterval)
                {
                    StartTimeInterval = 0;
                    EndTimeInterval = 1;
                }
                return;
            }
            ExitPlayMode = exitPlayMode;
            TargetFrame = singleFrame;
            CapFPS = capFPS;
        }
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
                    MyPreset.SetRecordModeToTimeInterval(StartTimeInterval, EndTimeInterval);
                    break;
            }
            MyPreset.FrameRatePlayback = Playback;

            return MyPreset;

        }
        /// <summary>
        /// Returns a list of RecorderSettings to be utilized in the Unity Editor
        /// </summary>
        /// <returns></returns>
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
                    blankImageRecorder.OutputFile = recData.OutputFileData;
                    movieImageRecorder.OutputFile = recData.OutputFileData;
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
                        blankAudioRecorder.OutputFile = recData.OutputFileData;
                        blankAudioRecorder.name = recData.RecorderName;
                        RecList.Add(blankAudioRecorder);
                    }
                    else
                    {
                        if(recData.RecorderType == FPRecorderType.AnimationClip)
                        {
                            blankAnimClipRecorder = recData.AnimationClipInputFormatData;
                            blankAnimClipRecorder.OutputFile = recData.OutputFileData;
                            blankAnimClipRecorder.name = recData.RecorderName;
                            RecList.Add(blankAnimClipRecorder);
                        }
                    }
                }
                

            }

            return RecList;
        }
        #endregion

        public (RecorderControllerSettings,bool) GenerateRecorderControllerSettingsForUnity(out string messages)
        {
            messages = "";
            RecorderNotes = "";
            var rcs = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            rcs.ExitPlayMode = ExitPlayMode;
            rcs.CapFrameRate = CapFPS;
            rcs.FrameRate = FrameRate;
            rcs.FrameRatePlayback = Playback;
            var recorderSettingsList = ReturnUnityRecorderByData();
            
            switch (RecordMode)
            {
                case RecordMode.Manual:
                    break;
                case RecordMode.SingleFrame:
                    rcs.SetRecordModeToSingleFrame(this.TargetFrame);
                    break;
                case RecordMode.FrameInterval:
                    rcs.SetRecordModeToFrameInterval(StartIntervalFrame, EndIntervalFrame);
                    break;
                case RecordMode.TimeInterval:
                    rcs.SetRecordModeToTimeInterval(StartTimeInterval, EndTimeInterval);
                    break;
            }
            
            try
            {
                bool listGood = false;
                if (recorderSettingsList.Count ==0)
                {
                    messages += "Recorder Settings List Count == 0";
                    RecorderNotes += "Recorder Settings List Count == 0";
                    listGood = false;
                }
                else
                {
                    messages += $"Recorder List: {recorderSettingsList.Count}";
                    RecorderNotes += $"Recorder List: {recorderSettingsList.Count}";
                    listGood = true;
                    //Debug.Log();
                    for (int i = 0; i < recorderSettingsList.Count; i++)
                    {
                        //Debug.Log($"Adding a Recorder from data: {recorderSettingsList[i].RecordMode}");
                        messages += "\n" + $"Adding a Recorder from data: {recorderSettingsList[i].RecordMode}";
                        RecorderNotes += "\n" + $"Adding a Recorder from data: {recorderSettingsList[i].RecordMode}";
                        var settingFile = recorderSettingsList[i];
                        rcs.AddRecorderSettings(settingFile);
                    }
                }

                return (rcs, listGood);
            }
            catch(Exception ex)
            {
                messages += ex.Message.ToString();
                return (null, false);
            }
            
        }
        /// <summary>
        /// Generic 360 Image Settings
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="imgFormat"></param>
        /// <param name="compType"></param>
        /// <param name="wCards"></param>
        /// <param name="camTag"></param>
        /// <param name="renderStereo"></param>
        /// <param name="flip"></param>
        /// <returns></returns>
        public FPRecorderDataStruct CreateImageSequence360PNG(int height, int width, int mapsize, ImageRecorderSettings.ImageRecorderOutputFormat imgFormat, ImageSource imgSource,ImageRecorderSettings.EXRCompressionType compType, List<FPWildCards> wCards, string camTag = "MainCamera", bool renderStereo = false, bool flip = false)
        {
            var newDataStruct = new FPRecorderDataStruct();
            //set our classifiers
            newDataStruct.RecorderType = FPRecorderType.ImageSequence;
            newDataStruct.Source = FPInputSettings.a360View;
            newDataStruct.Encoder = FPEncoderType.ImageEncoder;
            var inputSettings = new Camera360InputSettings();
            var outputFormat = new ImageRecorderSettings();

            //input
            inputSettings.OutputHeight = height;
            inputSettings.OutputWidth = width;
            inputSettings.MapSize = mapsize;
            inputSettings.RenderStereo = renderStereo;
            inputSettings.FlipFinalOutput = flip;
            inputSettings.Source = imgSource;
            inputSettings.CameraTag = camTag;
            //output format
            outputFormat.OutputFormat = imgFormat;
            outputFormat.EXRCompression = compType;
            //output file & set files to the 
            newDataStruct.OutputFileData = CreateOutputFileFromWildCards(wCards).FileName;
            newDataStruct.ThreeSixInputFormatData = inputSettings;
            newDataStruct.ImageSeqOutputFormatData = outputFormat;
            return newDataStruct;
        }
        private FP_OutputFileSO CreateOutputFileFromWildCards(List<FPWildCards> allWildCards)
        {
            var outputFileAsset = FP_OutputFileSO.CreateInstance(allWildCards, UnityEditor.Recorder.OutputPath.Root.AssetsFolder);
            return outputFileAsset;
        }

        /// <summary>
        /// Convert a GameObject to our Serialized struct
        /// </summary>
        /// <param name="cameraData"></param>
        public bool AddCameraData(GameObject cameraData)
        {
            int index = CameraPlacements.FindIndex(item => item.Tag == cameraData.tag);
            if (index == -1)
            {
                //add it
                FPTransformStruct newData = new FPTransformStruct()
                {
                    Tag = cameraData.tag,
                    Position = cameraData.transform.position,
                    Rotation = cameraData.transform.rotation.eulerAngles,
                    Name = cameraData.name,
                };
                CameraPlacements.Add(newData);
                return true;
            }
            else
            {
                return false;
            }
            
        }
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

        public string OutputFileData;

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
            data.OutputFileData = outputFileAsset.FileName;
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

    [Serializable]
    public struct FPTransformStruct
    {
        public string Tag;
        public string Name;
        public float3 Position;
        public float3 Rotation;
    }

}
