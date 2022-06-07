using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.DeviceMapper.Serialization;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.GUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper
{
    /// <summary>
    /// Responsible for savinf the MapperGUI Settings
    /// </summary>
    public class DeviceMapper : MonoBehaviour
    {
        /// <summary>
        /// Singleton reference
        /// </summary>
        public static DeviceMapper Instance
        {
            get { return _instance; }
            set
            {
                if (_instance != null)
                {
                    Destroy(value);
                    return;
                }
                _instance = value;
            }
        }

        [Header("********** User Settings ***********")]
        [Tooltip("")]
        [SerializeField]
        public KeyCode SettingsKeybind = KeyCode.Alpha1;
        /// <summary>
        /// The layer that the gameobjects will be added to when showing in the Preview window
        /// </summary>
        [Tooltip("")]
        public LayerMask CameraLayer;

        /// <summary>
        /// The tag that the gameobjects that need to be mapped have
        /// </summary>
        [Tooltip("")]
        public Tags GameobjectToMapTag;


        /// <summary>
        /// The multiplayer toggle in the GUI
        /// </summary>
        [SerializeField]
        [Header("********** End User Settings ***********")]
        [Space(20)]
        [Tooltip("")]
        private Toggle _multiplayerButton;


        public GameObject Mapper;
        public GameObject Debugger;

        /// <summary>
        /// A reference to its own <see cref="OverlayManager"/>
        /// </summary>
        public OverlayManager OverlayManager;

        /// <summary>
        /// Used to change the way the GUI and mapping work.
        /// <para>If <see cref="UsingMultiplayer"/> is true - each time the UDP connection stops receiving a <see cref="GenericDeviceData"/>, destroys its UnityObject (panel that represents the <see cref="GenericDeviceData"/> in the GUI). Looses all the <see cref="SingleInputGui"/> maping. </para>
        /// <para>If <see cref="UsingMultiplayer"/> is false - when the UDP connection stops receiving the last <see cref="GenericDeviceData"/> of its type (kinect, bitalino, etc) it dosen't destroy the UnityObject (panel that represents the <see cref="GenericDeviceData"/> in the GUI). Instead, puts the <see cref="GenericDeviceData"/> as null while waiting for a new one. Keeps the <see cref="SingleInputGui"/> mapping. </para>
        /// </summary>
        internal bool UsingMultiplayer;

        private static bool _iAlreadyExist;
        private static DeviceMapper _instance;

        protected void Awake()
        {
            if (_iAlreadyExist)
            {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += SceneLoadedCallback;
            _iAlreadyExist = true;


            Instance = this;
            DontDestroyOnLoad(this);

            Mapper.SetActive(true);
            Debugger.SetActive(true);
        }

        void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
        {
            DeviceMapperGuiManager.Instance.UpdateAvailableGameobjectsList(true);
            StartCoroutine(LoadSceneDefaultConfiguration(scene.name));
        }

        protected void Start()
        {
            _multiplayerButton.isOn = UsingMultiplayer;
            OverlayManager.ShowOrHideOverlay();
        }

        private IEnumerator LoadSceneDefaultConfiguration(string sceneName)
        {
            yield return null;
            print("Loading: " + "LastSave_" + sceneName);
            var lastSave = PlayerPrefs.GetString("LastSave_" + sceneName);

            if (lastSave != "")
            {
                MapperManager.ResetAllDeviceConnections(true);
                SaveLoadManager.Instance.LoadFileUsingPath(lastSave);
            }
        }
        
        void Update()
        {
            //transform.rotation *= Quaternion.Euler(1,0,0);
            if (Input.GetKeyDown(SettingsKeybind))
            {
                OverlayManager.ShowOrHideOverlay();
            }
        }

        /// <summary>
        /// Toggles the multiplayer option. Currently unavailable
        /// </summary>
        public void ToggleMultiplayer(Toggle toggle)
        {
            if (Time.frameCount < 2) return;
            UsingMultiplayer = toggle.isOn;
        }
    }
}
