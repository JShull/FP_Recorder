using System;
using UnityEngine;
using FuzzPhyte.Utility;
using UnityEditor;
using System.Collections;
using FuzzPhyte.Utility.Editor;
namespace FuzzPhyte.Recorder.Editor
{
    [Serializable]
    public static class FP_RecorderUtility
    {
        public const string PRODUCT_NAME = "FP_Recorder";
        public const string CAT0 = "Configuration";
        public const string CAT1 = "RecorderType";
        public const string CAT2 = "InputFile";
        public const string CAT3 = "OutputFormat";
        public const string CAT4 = "OutputFile";
        public const string CAT5 = "AddedRecorder";
        public const string SAMPLESPATH = "Assets/" + PRODUCT_NAME + "/Samples/URPSamples";
        public const string CamTAG = "FPCameraTag_";
        
        public static string ReturnProductName()
        {
            return PRODUCT_NAME;
        }

        public static  string ReturnSamplePath()
        {
            return SAMPLESPATH;
        }
    }

    /*
     * https://forum.unity.com/threads/control-unity-recorder-from-script.840946/
     * private RecorderWindow GetRecorderWindow()
        {
            return (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
        }
     **/
    [Serializable]
    public enum FPOutputResolution
    {
        MatchWindowSize = 0,
        a240p = 1,
        SD480p = 2,

    }
    [Serializable]
    public enum FPMediaFileFormat
    {
        PNG = 0,
        JPEG = 1,
        EXR = 2
    }
    [Serializable]
    public enum FPAudioFormat
    {
        WAV = 0,
    }
    [Serializable]
    public enum FPCompressionTypes
    {
        None = 0,
        RLE = 1,
        Zip = 2,
        PIZ = 3,
    }
    [Serializable]
    public enum FPAnimationCompression
    {
        Lossy = 0,
        Lossless = 1,
        Disabled = 2
    }
    [Serializable]
    public enum FPWildCards
    {
        AOV = 0,
        DATE = 1,
        EXTENSION = 2,
        FRAME = 3,
        GAMEOBJECT = 4,
        GAMEOBJECTSCENE = 5,
        PRODUCT = 6,
        PROJECT = 7,
        RECORDER = 8,
        RESOLUTION = 9,
        SCENE = 10,
        TAKE = 11,
        TIME = 12,
    }
    [Serializable]
    public enum FPInputSettings
    {
        GameView = 0,
        TargetedCamera = 1,
        a360View = 2,
        RenderTextureAsset = 3,
        TextureSampling = 4
    }
    [Serializable]
    public enum FPRecorderType
    {
        AnimationClip = 0,
        Movie = 1,
        ImageSequence = 2,
        Audio = 3
    }
    [Serializable]
    public enum FPEncoderType
    {
        ImageEncoder=0,
        UnityEncoder=1,
        ProResEncoder=2,
        GifEncoder=3
    }
    [Serializable]
    //https://docs.unity3d.com/Packages/com.unity.recorder@4.0/manual/OutputFileProperties.html#available-placeholders
    public static class RecorderPlaceholders
    {
        public const string AOV = "<AOV>";
        public const string DATE = "<Date>";
        public const string EXTENSION = "<Extension>";
        public const string FRAME = "<Frame>";
        public const string GAMEOBJECT = "<GameObject>";
        public const string GAMEOBJECTSCENE = "<GameObjectScene>";
        public const string PRODUCT = "<Product>";
        public const string PROJECT = "<Project>";
        public const string RECORDER = "<Recorder>";
        public const string RESOLUTION = "<Resolution>";
        public const string SCENE = "<Scene>";
        public const string TAKE = "<Take>";
        public const string TIME = "<Time>";

        public static string GetWildCardString(FPWildCards wCard)
        {
            switch (wCard)
            {
                case FPWildCards.AOV: return AOV;
                case FPWildCards.DATE: return DATE;
                case FPWildCards.EXTENSION: return EXTENSION;
                case FPWildCards.FRAME: return FRAME;
                case FPWildCards.GAMEOBJECT: return GAMEOBJECT;
                case FPWildCards.GAMEOBJECTSCENE: return GAMEOBJECTSCENE;
                case FPWildCards.PRODUCT: return PRODUCT;
                case FPWildCards.PROJECT: return PROJECT;
                case FPWildCards.RECORDER: return RECORDER;
                case FPWildCards.RESOLUTION: return RESOLUTION;
                case FPWildCards.SCENE: return SCENE;
                case FPWildCards.TIME: return TIME;
                case FPWildCards.TAKE: return TAKE;
                default: return TAKE;


            }
        }
    }
}
