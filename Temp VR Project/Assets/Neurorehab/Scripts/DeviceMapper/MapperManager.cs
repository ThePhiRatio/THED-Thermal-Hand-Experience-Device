using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper
{
    /// <summary>
    /// Responsible for managing all the <see cref="DeviceInputConnections"/> 
    /// </summary>
    public static class MapperManager
    {

        /// <summary>
        /// Backfield of the <see cref="ItemBeingDragged"/> property
        /// </summary>
        private static SingleInputGui _itemBeingDragged;

        /// <summary>
        /// Backfield of the <see cref="CurrentlySelectedInputGuiItem"/> property
        /// </summary>
        private static SingleInputGui _currentlySelectedInputGuiItem;

        /// <summary>
        /// The <see cref="SingleInputGui"/> that is currently being selected
        /// </summary>
        public static SingleInputGui CurrentlySelectedInputGuiItem
        {
            get { return _currentlySelectedInputGuiItem; }
            set { _currentlySelectedInputGuiItem = value; }
        }

        /// <summary>
        /// List of all the existent <see cref="SingleInputConnections"/>
        /// </summary>
        public static List<SingleInputConnections> DeviceInputConnections { get; set; }

        /// <summary>
        /// The <see cref="SingleInputGui"/> item that is being dragged in the GUI
        /// </summary>
        internal static SingleInputGui ItemBeingDragged
        {
            get { return _itemBeingDragged; }
            set
            {
                _itemBeingDragged = value;

                if (value != null)
                {
                    CurrentlySelectedInputGuiItem = value;
                    DeviceMapperGuiManager.Instance.ListAllSingleInputConections();
                    //DeviceMapperGuiManager.Instance.ListAllSingleInputInformationTypes();
                }
            }
        }

        /// <summary>
        /// private static converter
        /// </summary>
        static MapperManager()
        {
            DeviceInputConnections = new List<SingleInputConnections>();
        }

        /// <summary>
        /// Gets a <see cref="SingleInputConnections"/> from the <see cref="DeviceInputConnections"/> list
        /// </summary>
        /// <returns>Returns the <see cref="SingleInputConnections"/> list for the <see cref="CurrentlySelectedInputGuiItem"/> </returns>
        public static SingleInputConnections GetCurrentSingleInputConections()
        {
            return DeviceInputConnections.FirstOrDefault(i => i.SingleInput == CurrentlySelectedInputGuiItem.SingleInput);
        }

        /// <summary>
        /// Gets a <see cref="SingleInputConnections"/> from the <see cref="DeviceInputConnections"/> list
        /// </summary>
        /// <param name="siGui">The specified <see cref="SingleInputGui"/></param>
        /// <returns>Returns the <see cref="SingleInputConnections"/> list for the specified <see cref="SingleInputGui"/> </returns>
        public static SingleInputConnections GetSingleInputConections(SingleInputGui siGui)
        {
            return DeviceInputConnections.FirstOrDefault(i => i.SingleInput == siGui.SingleInput);
        }

        /// <summary>
        /// Connects a <see cref="GameObjectProperty"/> to a <see cref="SingleInputGui"/>. Adds it to the <see cref="DeviceInputConnections"/>  list.
        /// <para>Removes the last <see cref="SingleInputGui"/> that was mapped to that <see cref="GameObjectProperty"/>, if it is the case.</para>
        /// <para>Also Updates the GUI</para>
        /// </summary>
        /// <param name="gameObjectPropertyGui">The <see cref="GameObjectPropertyGui"/> that has the <see cref="GameObjectProperty"/></param>
        public static void TriggerGameObjectPropertyActivation(GameObjectPropertyGui gameObjectPropertyGui)
        {
            //activate SingleInput
            ItemBeingDragged.SingleInput.Activated = true;

            //remove from old single inputs if they exist
            RemoveAllGameobjectPropertyMappings(gameObjectPropertyGui.GameObjectProperty);

            //update gameobjectProperty references
            var gameObjecrProp = gameObjectPropertyGui.GameObjectProperty;
            gameObjecrProp.Target = DeviceMapperGuiManager.Instance.CurrentSelectedObject.Target;

            //reference to singleInputMappings in gameobjectProp
            gameObjecrProp.ResetSingleInputMappingsList();
            gameObjecrProp.UpdateSingleInputMappingsList(ItemBeingDragged.SingleInput);

            //update edit button in GUI
            gameObjectPropertyGui.UpdateGopGui();

            AddSingleInputConnection(gameObjecrProp);

            //map to the current selected Gameobject
            gameObjectPropertyGui.StartMapping();

            //gui
            DeviceMapperGuiManager.Instance.ListAllSingleInputConections();
            DeviceMapperGuiManager.Instance.ListAllSelectedGameobjectProperties();

            ItemBeingDragged = null;
        }

        /// <summary>
        /// Connects a <see cref="GameObjectProperty"/> to a <see cref="SingleInputGui"/>. Adds it to the <see cref="DeviceInputConnections"/>  list.
        /// <para>Removes the last <see cref="SingleInputGui"/> that was mapped to that <see cref="GameObjectProperty"/>, if it is the case.</para>
        /// <para>Also Updates the GUI</para>
        /// </summary>
        /// <param name="gameObjectPropertyGui">The <see cref="GameObjectPropertyGui"/> that has the <see cref="GameObjectProperty"/></param>
        /// <param name="axis"></param>
        public static void TriggerMiniGameObjectPropertyActivation(GameObjectPropertyGui gameObjectPropertyGui,
            AxisLabels axis)
        {
            var gop = gameObjectPropertyGui.GameObjectProperty;

            //activate SingleInput
            ItemBeingDragged.SingleInput.Activated = true;
            
            switch (gop.InfoType)
            {
                case InformationType.rotation:
                    gameObjectPropertyGui.GameObjectProperty.UpdateSpecificSingleInputMappingRotation(ItemBeingDragged.SingleInput, axis);
                    break;
                case InformationType.position:
                    gameObjectPropertyGui.GameObjectProperty.UpdateSpecificSingleInputMappingPosition(ItemBeingDragged.SingleInput, axis);
                    break;
                default:
                    TriggerGameObjectPropertyActivation(gameObjectPropertyGui);
                    return;
            }
            
            //map to the current selected Gameobject
            gameObjectPropertyGui.StartMapping();

            //activate mini axis - color orange
            gameObjectPropertyGui.UpdateGopGui();

            AddSingleInputConnection(gop);

            //gui
            DeviceMapperGuiManager.Instance.ListAllSingleInputConections();
            DeviceMapperGuiManager.Instance.ListAllSingleInputConections();

            ItemBeingDragged = null;
        }

        /// <summary>
        /// Add the <see cref="GameObjectProperty"/> to the <see cref="DeviceInputConnections"/> list. Creates a new connection or adds th <see cref="GameObjectProperty"/> to an existent <see cref="SingleInput"/>
        /// </summary>
        /// <param name="gameObjecrProp"></param>
        private static void AddSingleInputConnection(GameObjectProperty gameObjecrProp)
        {
            //add to new single input
            var singleInputGameobjects = DeviceInputConnections.FirstOrDefault(i => i.SingleInput == ItemBeingDragged.SingleInput);

            //if there already exist the selected single input inside the DeviceInputConnections
            if (singleInputGameobjects != null && singleInputGameobjects.ObjectProperties.Contains(gameObjecrProp) == false)
            {
                singleInputGameobjects.ObjectProperties.Add(gameObjecrProp);
            }

            //else creates a new SingleInputGameobjects and adds to DeviceInputConnections list of the MapperManager
            else
            {
                DeviceInputConnections.Add(new SingleInputConnections(ItemBeingDragged.SingleInput, new List<GameObjectProperty>
                {
                    gameObjecrProp
                }));
            }
        }

        /// <summary>
        /// Removes a  specified <see cref="GameObjectProperty"/> from its mapped <see cref="SingleInputGui"/> in the <see cref="DeviceInputConnections"/> list
        /// <para>Also Updates the GUI</para>
        /// </summary>
        /// <param name="gameObjectProperty">The <see cref="GameObjectPropertyGui"/> that has the <see cref="GameObjectProperty"/> to remove</param>
        /// <param name="singleInp"></param>
        public static void RemoveSpecificGameobjectPropertyFromSingleInput(GameObjectProperty gameObjectProperty, SingleInputGui singleInp = null)
        {
            if (singleInp == null)
                singleInp = CurrentlySelectedInputGuiItem;
            
            //get all the mappings that use this singleInput and remove them
             var mappings = gameObjectProperty.SingleInputMappings.Where(
                con => con.SingleInput == singleInp.SingleInput);

            foreach (var singleInputMapping in mappings)
            {
                singleInputMapping.SingleInput = new SingleInput(null, "");
                singleInputMapping.InputMappingLabels = SingleInputMappingLabels.None;
            }

            //check if it needs to stop map (if there are no singleInputmappings with labels different than none
            if (gameObjectProperty.SingleInputMappings.All(sim => sim.InputMappingLabels == SingleInputMappingLabels.None))
                gameObjectProperty.StopMapping();

            //remove from devices conections list
            RemovePropertyFromDevicesConnections(gameObjectProperty, GetSingleInputConections(singleInp));
        }

        /// <summary>
        /// Removes the <see cref="GameObjectProperty"/>from any mappings in the <see cref="SingleInputConnections"/> and its <see cref="GameObjectProperty.SingleInputMappings"/>
        /// </summary>
        /// <param name="gameObjectProperty">The <see cref="GameObjectPropertyGui"/> that has the <see cref="GameObjectProperty"/> to remove</param>
        public static void RemoveAllGameobjectPropertyMappings(GameObjectProperty gameObjectProperty)
        {
            foreach (var singleInputMapping in gameObjectProperty.SingleInputMappings)
            {
                //if it is the first time it is being added
                if (singleInputMapping.SingleInput.SingleInputGui == null) continue;

                var curSingleInputConnections =
                    GetSingleInputConections(singleInputMapping.SingleInput.SingleInputGui);

                if (curSingleInputConnections == null) continue;

                //remove from devices conections list
                RemovePropertyFromDevicesConnections(gameObjectProperty, curSingleInputConnections);
            }
            
            //stops any binds to interpreters and resets SingleInputMapping list
            gameObjectProperty.StopMapping();
        }

        /// <summary>
        /// Removes the <see cref="GameObjectProperty"/> from the received <see cref="SingleInputConnections"/>
        /// </summary>
        /// <param name="gameObjectProperty"><see cref="GameObjectProperty"/> to remove</param>
        /// <param name="curSingleInputConnections"><see cref="SingleInputConnections"/> to remove from</param>
        private static void RemovePropertyFromDevicesConnections(GameObjectProperty gameObjectProperty,
            SingleInputConnections curSingleInputConnections)
        {
            //remove gameobjectProp from the DeviceConnections
            curSingleInputConnections.ObjectProperties.Remove(gameObjectProperty);

            if (curSingleInputConnections.ObjectProperties.Count != 0) return;

            //if there are no more connections to the specified SingleInput
            curSingleInputConnections.SingleInput.Activated = false;
            DeviceInputConnections.Remove(curSingleInputConnections);
        }

        /// <summary>
        /// Clears the specified <see cref="SingleInputGui"/> connections list removing its <see cref="SingleInputConnections"/> from the <see cref="DeviceInputConnections"/> list.
        /// <para>Also Updates the GUI</para>
        /// </summary>
        /// <param name="singleInputGui">The <see cref="SingleInputGui"/> to clear the connections from</param>
        public static void ClearSingleInputConnections(SingleInputGui singleInputGui)
        {
            var singleInputGameobjects = GetSingleInputConections(singleInputGui);

            if (singleInputGameobjects == null) return; //there are no connections in this SIngleInput

            //removes all Gameobjectproperties from the DevicesInputConnections list
            foreach (var selectedObjectProperty in singleInputGameobjects.ObjectProperties.ToList())
            {
                RemoveSpecificGameobjectPropertyFromSingleInput(selectedObjectProperty,
                    singleInputGameobjects.SingleInput.SingleInputGui);
            }
            
            //gui
            DeviceMapperGuiManager.Instance.ClearSingleInputConnectionsList();
            DeviceMapperGuiManager.Instance.ListAllSelectedGameobjectProperties();
        }

        /// <summary>
        /// Resets all the <see cref="SingleInputConnections"/> existing in <see cref="DeviceInputConnections"/>
        /// </summary>
        /// <param name="reset"></param>
        public static void ResetAllDeviceConnections(bool reset)
        {
            if (reset == false) return;

            //if (DeviceMapperGuiManager.Instance.CurrentSelectedObject == null && forceReset == false) return;

            var connetions = DeviceInputConnections.ToList();

            foreach (var singleInputConnections in connetions)
            {
                ClearSingleInputConnections(singleInputConnections.SingleInput.SingleInputGui);
            }

            DeviceInputConnections.Clear();

            foreach (Transform goiTransform in DeviceMapperGuiManager.Instance.AvailableGameobjectsPanel.transform)
            {
                var goi = goiTransform.GetComponent<GameObjectInformation>();
                foreach (var gop in goi.ObjectProperties)
                {
                    if (gop.Active == false) continue;
                    DeviceMapperGuiManager.Instance.RemoveGameobjectProperty(goi, gop.InfoType);
                }
            }

            DeviceMapperGuiManager.Instance.ClearSingleInputConnectionsList();
            DeviceMapperGuiManager.Instance.ListAllSelectedGameobjectProperties();
            //DeviceMapperGuiManager.Instance.RemoveSelectedObject();
        }
    }
}
