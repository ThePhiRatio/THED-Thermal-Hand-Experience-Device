using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Represents a <see cref="GameObjectProperty"/> in the GUI
    /// </summary>
    [RequireComponent(typeof(DropHandler))]
    public class GameObjectPropertyGui : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// This gameobject Edit Button
        /// </summary>
        private Button _editButton;

        /// <summary>
        /// This gameobject Dropdown Button
        /// </summary>
        private GameObject _arrowDropdownButton;

        /// <summary>
        /// The <see cref="GameObjectProperty"/> reference
        /// </summary>
        public GameObjectProperty GameObjectProperty { get; set; }

        /// <summary>
        /// The list of axis inside the gameobject Property Gui, if they exist
        /// </summary>
        public List<GameObject> GoPGuiAxis;

        /// <summary>
        /// The initial color of the gameobject in the GUI
        /// </summary>
        private Color _initColor;

        /// <summary>
        /// The color of the gameobject in the GUI that is shown when the <see cref="GameObjectProperty"/> is active
        /// </summary>
        private Color _activeColor;

        /// <summary>
        /// Refrences its own <see cref="GameObjectProperty"/> and updates its text in the GUI according to being connected to a <see cref="SingleInputGui"/> or not
        /// </summary>
        /// <param name="gameObjectProperty"></param>
        public void Init(GameObjectProperty gameObjectProperty)
        {
            _editButton = transform.Find("SideBtns").Find("EditBtn").GetComponent<Button>();
            _arrowDropdownButton = transform.Find("SideBtns").Find("DropdownBtn").gameObject;
            _arrowDropdownButton.SetActive(false);
            GameObjectProperty = gameObjectProperty;
            GameObjectProperty.GameObjectPropertyGui = this;

            _initColor = GetComponent<Image>().color;
            _activeColor = new Color32(255, 136, 0, 255);

            UpdateGopGui();
            UpdateName();

            if (GameObjectProperty.InfoType == InformationType.value || GameObjectProperty.InfoType == InformationType.@bool || GameObjectProperty.InfoType == InformationType.sample) return;

            _arrowDropdownButton.SetActive(true);
        }

        /// <summary>
        /// Updates the color and the edit button of each axis in the gui
        /// </summary>
        private void UpdateAllAxis()
        {
            for (var i = 0; i < GoPGuiAxis.Count; i++)
            {
                var active = GameObjectProperty.SingleInputMappings[i].InputMappingLabels != SingleInputMappingLabels.None;
                GoPGuiAxis[i].GetComponent<Image>().color = active ? _activeColor : _initColor;
                GoPGuiAxis[i].transform.Find("SideBtns").Find("EditBtn").GetComponent<Button>().interactable = active;
            }
        }

        /// <summary>
        /// Updates the UI according to the Active property in <see cref="GameObjectProperty"/>
        /// </summary>
        public void UpdateGopGui()
        {
            var active = GameObjectProperty.Active && GameObjectProperty.SingleInputMappings.Any(sim => sim.InputMappingLabels != SingleInputMappingLabels.None);
            
            _editButton.interactable = active;

            gameObject.GetComponent<Image>().color = active ? _activeColor : _initColor;

            if (GameObjectProperty.InfoType == InformationType.value || GameObjectProperty.InfoType == InformationType.@bool || GameObjectProperty.InfoType == InformationType.sample) return;

            UpdateAllAxis();

        }

        /// <summary>
        /// Updates the text in the GUI. If the <see cref="GameObjectProperty"/> does not have a <see cref="SingleInputGui"/> mapped then it just show its <see cref="InformationType"/>, otherwise shows also the <see cref="SingleInputGui"/> label that is mapped to. 
        /// </summary>
        public void UpdateName()
        {
            string name = GameObjectProperty.InfoType.ToString();

            foreach (var singleInputMapping in GameObjectProperty.SingleInputMappings)
            {
                if (singleInputMapping.SingleInput == null || singleInputMapping.SingleInput.GenericDeviceData == null) continue;
                name += " " + singleInputMapping.SingleInput.GenericDeviceData.DeviceName + " " + singleInputMapping.SingleInput.Label + " "  + singleInputMapping.InputMappingLabels;
            }

            GetComponentInChildren<Text>().text = name;
        }

        /// <summary>
        /// Updates the text in the GUI and starts mapping the gamobject interpreter
        /// </summary>
        public void StartMapping()
        {
            UpdateName();
            GameObjectProperty.StartMapping();
        }

        /// <summary>
        /// Updates the text in the GUI and stops mapping the gameobject interpreter
        /// </summary>
        public void StopMapping()
        {
            UpdateName();
            GameObjectProperty.StopMapping();
        }

        /// <summary>
        /// If right mouse clicked, clear this <see cref="GameObjectProperty"/> from all <see cref="SingleInput"/> associated in the <see cref="MapperManager.DeviceInputConnections"/> 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerId == -2) //right click -->
            {
                MapperManager.RemoveAllGameobjectPropertyMappings(GameObjectProperty);

                //gui
                DeviceMapperGuiManager.Instance.ListAllSingleInputConections();
                DeviceMapperGuiManager.Instance.ListAllSelectedGameobjectProperties();
            }
            else if (eventData.pointerId == -1)
            {
                if(_editButton.interactable == false)
                    MessageManager.Instance.ShowPopupMessage("You need an input connected to it before beig able to calibrate/edit.");
            }
        }

        /// <summary>
        /// Shows in the GUI the Editor Window for the specified <see cref="GameObjectProperty"/> type
        /// <para>Triggered when the calibrate button of the mini property is clicked</para>
        /// </summary>
        /// <param name="go">The <see cref="GameObjectPropertyGui"/> gameobject pressed</param>
        public void ShowOneAxisPropertyEditorWindow(GameObject clickedBtn)
        {
            var axisIndexClicked = GoPGuiAxis.IndexOf(clickedBtn);

            PropertyEditorGuiManager.Instance.ShowSinglePropertyEditorWindow(this, axisIndexClicked);
        }
    }
}
