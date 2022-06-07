 using Neurorehab.Scripts.CpDebugger.Gui;
 using Neurorehab.Scripts.DeviceMapper.Gui;
 using Neurorehab.Scripts.Udp;
 using Neurorehab.Scripts.Utilities;
 using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.GUI
{
    /// <summary>
    /// Responsible for Managing an overlay window that uses UDP. Used together with the system manager, for example <see cref="Neurorehab.Scripts.DeviceMapper"/> or <see cref="Neurorehab.Scripts.CpDebugger"/>
    /// </summary>
    public class OverlayManager : MonoBehaviour
    {
        public bool MapperOverlay;
        /// <summary>
        /// DeviceMapper Canvas
        /// </summary>
        [SerializeField]
        [Tooltip("")]
        private Canvas _overlayCanvas;

        /// <summary>
        /// The Udp receiver reference
        /// </summary>
        [SerializeField]
        private UdpReceiver _udpReceiver;

        /// <summary>
        /// A reference to the Udp Port Input Field in the Gui
        /// </summary>
        [SerializeField]
        private InputField _portInputField;

        /// <summary>
        /// A reference to the Udp Start/Stop toggle in the Gui
        /// </summary>
        [SerializeField]
        private Toggle _udpStartToggle;
        /// <summary>
        /// A reference to the Udp Start/Stop toggle in the Gui
        /// </summary>
        [SerializeField]
        private Toggle _udpSecundaryStartToggle;


        /// <summary>
        /// Overlay Canvas
        /// </summary>
        public Canvas OverlayCanvas
        {
            get { return _overlayCanvas; }

            set { _overlayCanvas = value; }
        }
        
        /// <summary>
        /// A reference to the IP label in the GUI
        /// </summary>
        [SerializeField]
        private Text _IpLabel;

        private Canvas _canvas;

        protected void Awake()
        {
            //_udpReceiver.Init();
            _udpStartToggle.isOn = true;

            _IpLabel.text = Utilities.Network.GetLocalIP() + ":";

            UpdateCanvas();

            //CpDebuggerGuiManager.Instance.IsHidden = !_canvas.enabled;
        }

        private void UpdateCanvas()
        {
            if (OverlayCanvas == null)
                OverlayCanvas = GetComponentInChildren<Canvas>();

            _canvas = OverlayCanvas.gameObject.GetComponent<Canvas>();
        }

        /// <summary>
        /// Shows or hides the <see cref="OverlayCanvas"/> when clicked
        /// </summary>
        public void ShowOrHideOverlay()
        {
            if (PauseOnEnable.Instance == null)
                return;
            UpdateCanvas();
            _canvas.enabled = !_canvas.enabled;

            PauseOnEnable.Instance.PauseBasedOnScale(_canvas);

            if (MapperOverlay)
            {
                if(_canvas.enabled == false)
                    DeviceMapperGuiManager.Instance.ResetGameobjectLayer();

                DeviceMapperGuiManager.Instance.IsShowing = _canvas.enabled;
            }
            else
                CpDebuggerGuiManager.Instance.IsHidden = !_canvas.enabled;
        }
        
        /// <summary>
        /// Updates the Udp port
        /// </summary>
        public void UdpPortChanged()
        {
            if (_portInputField.text != "")
                _udpReceiver.Port = int.Parse(_portInputField.text);
        }

        /// <summary>
        /// Starts or stops the UdpReceiver thread
        /// </summary>
        public void StartOrStopUdpReceiver()
        {
            if (_udpStartToggle.isOn)
            {
                _portInputField.interactable = false;
                _udpReceiver.Init();
                _udpStartToggle.GetComponentInChildren<Text>().text = "Disconnect";

                if(_udpSecundaryStartToggle != null)
                    _udpSecundaryStartToggle.isOn = true;

            }
            else
            {
                _portInputField.interactable = true;
                _udpReceiver.Stop();
                _udpStartToggle.GetComponentInChildren<Text>().text = "Connect";

                    if (_udpSecundaryStartToggle != null)
                    _udpSecundaryStartToggle.isOn = false;
            }
        }
    }
}