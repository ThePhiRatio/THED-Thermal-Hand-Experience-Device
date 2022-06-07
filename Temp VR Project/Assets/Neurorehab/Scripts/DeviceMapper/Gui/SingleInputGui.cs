using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Serialization;
using Neurorehab.Scripts.Devices.Abstracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Represents a <see cref="GenericDeviceData"/> dictionary entry in the GUI
    /// </summary>
    [RequireComponent(typeof(DragHandler))]
    public class SingleInputGui : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        /// <summary>
        /// The <see cref="SingleInput"/> this belongs too
        /// </summary>
        public SingleInput SingleInput { get; set; }
        
        /// <summary>
        /// The initial color of the gameobject in the GUI
        /// </summary>
        private Color _initColor;

        /// <summary>
        /// The last color of the gameobject in the GUI
        /// </summary>
        private Color _lastColor;

        /// <summary>
        /// The key of the specific entry in the <see cref="GenericDeviceData"/> Dictionaries.
        /// </summary>
        public string Label;

        /// <summary>
        /// Outline of the SIngleInputGui. Used on muse over/exit
        /// </summary>
        private Image _img;


        private void Awake()
        {
            _img = GetComponent<Image>();
            _initColor = GetComponent<Image>().color;
            _lastColor = _initColor;

            SingleInput = new SingleInput(this, Label);


            SaveLoadData.AddSingleInput(SingleInput);
            //print(Label);
        }

        private void Start()
        {
            StartCoroutine(StartUpdatingInformationTypes());
        }

        /// <summary>
        /// If right mouse clicked, clear all singleinput connections.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            MapperManager.CurrentlySelectedInputGuiItem = this;

            if (eventData.pointerId == -2) //right click -->
            {
                MapperManager.ClearSingleInputConnections(this);
            }
            else if (eventData.pointerId == -1)
            {
                MapperManager.CurrentlySelectedInputGuiItem = this;
                DeviceMapperGuiManager.Instance.ListAllSingleInputConections();
                //DeviceMapperGuiManager.Instance.ListAllSingleInputInformationTypes();
            }
        }

        /// <summary>
        /// Coroutine that calls <see cref="SingleInput.UpdateInformationTypes"/> 
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartUpdatingInformationTypes()
        {
            SingleInput.Label = Label;
            while (true)
            {
                yield return new WaitForSecondsRealtime(.1f);

                if (SingleInput == null || SingleInput.GenericDeviceData == null)
                    continue;

                SingleInput.UpdateInformationTypes();

                //if (SingleInput.InfoTypes.Contains(InformationType.unknown))
                //    GetComponent<CanvasGroup>().interactable = false;
            }
        }


        /// <summary>
        /// Hides/Unhides the gameobject according to the udp connection (hides if not receiving) received from <see cref="SingleInput.Connected"/>. Puts its parent red in the GUI if it is not receiving.
        /// </summary>
        public void ShowOrHideGameobject()
        {
            var isConnected = SingleInput.Connected();
            gameObject.GetComponent<CanvasGroup>().alpha = isConnected ? 1 : 0;
            //gameObject.GetComponent<Image>().color = isConnected ? _initColor : Color.red;

            if (transform.parent == gameObject.GetComponent<DragHandler>().InitialParent)
                transform.parent.GetComponent<Image>().color = isConnected ? _initColor : Color.red;
        }

        /// <summary>
        /// Activates the <see cref="SingleInput"/> in the GUI according the the boolean received
        /// </summary>
        /// <param name="activate"></param>
        public void UpdateActivation(bool activate)
        {
            _img.color = activate ? Color.green : _initColor;

            if (transform.parent == gameObject.GetComponent<DragHandler>().InitialParent)
                transform.parent.GetComponent<Image>().color = activate ? Color.green : _initColor;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<DragHandler>().ResetPosition();
        }

        /// <summary>
        /// Mouseover enter function for the SingleInputGui buttons
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _lastColor = _img.color;
            _img.color = new Color32(255, 136, 0, 255);
        }

        /// <summary>
        /// Mouseover exit function for the SingleInputGui buttons
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            _img.color = _lastColor;
        }
    }
}
