using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using FuzzPhyte.Recorder.Editor;
using FuzzPhyte.Utility.Editor;
#endif
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
namespace FuzzPhyte.Recorder
{
    public class FPRecorderCam : MonoBehaviour
    {
        #region Camera SpawnKeys
        [Header("Save JSON When You Stop?")]
        [Tooltip("This will also backup the Unity Recorder internally - quick fix to me removing the recorder cache constantly")]
        protected bool SaveRecorderAssetOnEnd = true;
        [Header("Camera Information")]
        public KeyCode SpawnCamKey = KeyCode.C;
        public GameObject CameraPrefab;
        private bool passedParent;
        public Canvas UICamCanvasRef;
        public TextMeshProUGUI CamCount;
        public TextMeshProUGUI Elevation;
        public TextMeshProUGUI UserFileNameRef;
        [SerializeField]
        [Tooltip("Placeholder for cameras we generate/spawn at runtime")]
        private List<GameObject> CameraList = new List<GameObject>();
        public int MaxCamerasInScene = 25;
        [Tooltip("Color when already in the scene")]
        public Color PlacedColor = Color.blue;
        [Tooltip("Color during runtime place")]
        public Color ActivePlacedRuntime = Color.red;
        #endregion
        #region Motion Keys
        [Header("Key Bindings")]
        [Space]
        public KeyCode ForwardMotion = KeyCode.W;
        public KeyCode BackwardMotion = KeyCode.S;
        public KeyCode RightMotion = KeyCode.D;
        public KeyCode LeftMotion = KeyCode.A;
        public KeyCode ElevationUp = KeyCode.E;
        public KeyCode ElevationDown = KeyCode.Q;
        [Space]
        public UnityEvent OnCamSpawn;
        [Space]
        [Header("Camera Motion Parameters")]
        #endregion
#region Camera Speed Parameters 
        public float speed = 10.0f; // Speed of camera movement
        public float sensitivity = 0.1f; // Mouse movement sensitivity
        public float elevationSpeed = 2.0f; // Speed of elevation change
        public float MinPitch = -89;
        public float MaxPitch = 89;
        private float yaw = 0.0f; // Yaw to store the angle in the horizontal plane
        private float pitch = 0.0f; // Pitch to store the angle in the vertical plane
        private int startingCamCount;
        #endregion
        #region Unity Functions
        public void Start()
        {
#if UNITY_EDITOR
            //go and check for cameras that have tags already in scene and update the list
            for (int i = 0; i < MaxCamerasInScene; i++)
            {
                var tagToFind = FP_RecorderUtility.CamTAG + (i).ToString();
                //check if it exists
                if (FPGenerateTag.DoesTagExist(tagToFind))
                {
                    var aPotentialCam = GameObject.FindGameObjectWithTag(tagToFind);
                    if (aPotentialCam != null)
                    {
                        CameraList.Add(aPotentialCam);
                    }
                }
                else
                {
                    //Debug.LogWarning($"Missing the tag: {FP_RecorderUtility.CamTAG + (i+1).ToString()}, did you need to add them?");
                    FPGenerateTag.CreateTag(tagToFind);
                    Debug.Log($"Created a tag: {tagToFind}");
                    var aPotentialCam = GameObject.FindGameObjectWithTag(tagToFind);
                    if (aPotentialCam != null)
                    {
                        CameraList.Add(aPotentialCam);
                        if (aPotentialCam.transform.GetChild(0).GetComponent<MeshRenderer>())
                        {
                            aPotentialCam.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = PlacedColor;
                        }
                    }
                }
            }
            //turn on mesh renderers if we have them and turn off the camera component
            foreach (var cam in CameraList)
            {
                if (cam.GetComponent<Camera>())
                {
                    cam.GetComponent<Camera>().enabled = false;
                }
                if (cam.transform.GetChild(0).GetComponent<MeshRenderer>())
                {
                    cam.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                }
            }
            //
            startingCamCount = CameraList.Count;
            FPPreserveObjects.Instance.UpdateStartingCamCount(startingCamCount);
            FPPreserveObjects.Instance.SaveJSONUpdate(SaveRecorderAssetOnEnd);
            //need to force the tag manager to save
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            tagManager.ApplyModifiedProperties();
            //update the UI
            CamCount.text = CameraList.Count.ToString();
            FPPreserveObjects.Instance.UpdateCamColor(PlacedColor);
            UpdateUI();
            //update the Editor Recorder with camera information
#endif
        }
        
        
        public void Update()
        {
            #if UNITY_EDITOR
            MoveCam();
            if(Input.GetKeyDown(SpawnCamKey)){
                SpawnCam();
            }
            
            //round to two decimals
            UpdateUI();
            #endif
        }
        #endregion
        #region Camera Functions
        void MoveCam()
        {
#if UNITY_EDITOR
            // Mouse movement for rotation
            // if using new input system
#if ENABLE_INPUT_SYSTEM
            yaw += sensitivity * Mouse.current.delta.x.ReadValue();
            pitch -= sensitivity * Mouse.current.delta.y.ReadValue();
#elif ENABLE_LEGACY_INPUT_MANAGER
            yaw += sensitivity * Input.GetAxis("Mouse X");
            pitch -= sensitivity * Input.GetAxis("Mouse Y");
#endif
            pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch); // Clamp the pitch angle to prevent flipping at the poles

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

            // Movement vector based on input and camera orientation, ignoring y to maintain elevation
            float zMove = 0;
            float xMove = 0;
            if (Input.GetKey(ForwardMotion))
            {
                zMove = 1;
            }
            if (Input.GetKey(BackwardMotion))
            {
                zMove = -1;
            }
            if (Input.GetKey(RightMotion))
            {
                xMove = 1;
            }
            if (Input.GetKey(LeftMotion))
            {
                xMove = -1;
            }
            Vector3 move = new Vector3(xMove, 0, zMove);
            move = transform.TransformDirection(move);
            move.y = 0; // Zero out y movement to maintain elevation

            transform.position += move * speed * Time.deltaTime;

            // Elevation changes with Plus and Minus
            if (Input.GetKeyDown(ElevationUp)) // Plus key (Equals key without shift usually)
            {
                transform.position += Vector3.up * elevationSpeed;
            }
            if (Input.GetKeyDown(ElevationDown)) // Minus key
            {
                transform.position += Vector3.down * elevationSpeed;
            }
            #endif
        }
        void SpawnCam()
        {
#if UNITY_EDITOR
            if (!passedParent)
            {
                passedParent = true;
                //FPPreserveObjects.Instance.AddParentItem(ParentItem);
                FPPreserveObjects.Instance.UpdateCamerasInScene(CameraList.Count);
            }
            int tagCountInt = CameraList.Count;
            if (MaxCamerasInScene <= tagCountInt)
            {
                Debug.LogWarning("Max Cameras in Scene Reached - make sure to increase the value before you add more cameras - then replay this after you do that! (going to force a stop on playtime)");
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }
                return;
            }
            OnCamSpawn.Invoke();
            Debug.Log($"Tag Count = {tagCountInt}");
            var camTag = FP_RecorderUtility.CamTAG + tagCountInt.ToString();
            //FPMenu.RuntimeAddSingleTag();
            GameObject cam = Instantiate(CameraPrefab, this.transform.position, Quaternion.identity);
            if (cam.transform.GetChild(0)) {
                cam.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = ActivePlacedRuntime;
            }
            cam.tag = camTag;
            FPPreserveObjects.Instance.AddObjectForTracking(cam);
            FPPreserveObjects.Instance.AddTagIndex(camTag, tagCountInt);
            FPPreserveObjects.Instance.AddTag(camTag);
            CameraList.Add(cam);
            UpdateUI();
#endif
            //FPMenu.RunTimeAddCamera(tagCountInt);
        }
        #endregion
        #region UI Updates

        public void UIInputChanged()
        {
#if UNITY_EDITOR
            FPPreserveObjects.Instance.UpdateUserFileName(UserFileNameRef.text);
            #endif
        }
        public void UIToggleChange()
        {
            SaveRecorderAssetOnEnd = !SaveRecorderAssetOnEnd;
#if UNITY_EDITOR
            FPPreserveObjects.Instance.SaveJSONUpdate(SaveRecorderAssetOnEnd);
            #endif
        }
        private void UpdateUI(){
            CamCount.text = $"C: {CameraList.Count.ToString()} | {MaxCamerasInScene-CameraList.Count} left";
            Elevation.text = $"H: {transform.position.y.ToString("F2")}";
        }
        /// <summary>
        /// This button script will match scene cameras to the recorder - it is a hard reset on the settings data 
        /// </summary>
        public void UIButtonMatchSceneCamerasRecorder()
        {
#if UNITY_EDITOR
            FPMenu.RuntimeResetSettings();
            //clear preservedObject data
            FPPreserveObjects.Instance.ResetPreservedValues();
            FPPreserveObjects.Instance.UpdateCamerasInScene(CameraList.Count);
            for (int i = 0; i < CameraList.Count; i++)
            {
                var cam = CameraList[i];
                var camTag = cam.tag;
                var tagCountInt = i;
                FPPreserveObjects.Instance.AddObjectForTracking(cam);
                FPPreserveObjects.Instance.AddTagIndex(camTag, tagCountInt);
                FPPreserveObjects.Instance.AddTag(camTag);
            }
            UpdateUI();
            #endif
        }
        #endregion
    }
}
