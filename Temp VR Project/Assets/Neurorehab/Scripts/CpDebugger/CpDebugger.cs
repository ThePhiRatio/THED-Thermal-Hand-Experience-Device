using Neurorehab.Scripts.GUI;
using UnityEngine;

namespace Neurorehab.Scripts.CpDebugger
{
    /// <summary>
    /// Responsible for managing the CPDebugger System in Unity
    /// </summary>
    public class CpDebugger : MonoBehaviour
    {
        /// <summary>
        /// Backingfield of <see cref="SettingsKeybind"/> Keybind used to show/hide the CPDebugger interface
        /// </summary>
        [Header("********** User Settings ***********")]
        [Tooltip("")]
        [SerializeField]
        private KeyCode _settingsKeybind = KeyCode.F1;

        /// <summary>
        /// A reference to its own <see cref="OverlayManager"/>
        /// </summary>
        [Header("********** End User Settings ***********")]
        public OverlayManager OverlayManager;

        private void Start()
        {
            //OverlayManager.ShowOrHideOverlay();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(_settingsKeybind))
            {
                OverlayManager.ShowOrHideOverlay();
            }
        }
    }
}
