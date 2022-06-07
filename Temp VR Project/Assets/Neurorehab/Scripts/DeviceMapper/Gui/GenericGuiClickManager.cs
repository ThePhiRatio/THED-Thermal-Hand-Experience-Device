using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for the clicks in the DeviceMapper GUI
    /// </summary>
    public class GenericGuiClickManager : MonoBehaviour
    {
        /// <summary>
        /// Redirects to the <see cref="MapperManager"/> and removes the Selected <see cref="GameObjectProperty"/> from the current selected <see cref="SingleInputGui"/>
        /// <para>Triggered when a gamobjectProp is removed in the GUI (clicked in the X button) from the Selected Single Input</para>
        /// </summary>
        /// <param name="go"></param>
        public void RemoveSingleInputConnection(GameObject go)
        {
            var goProp = go.GetComponent<GameObjectPropertyGui>();
            
            MapperManager.RemoveSpecificGameobjectPropertyFromSingleInput(goProp.GameObjectProperty);
            
            goProp.UpdateName();

            //gui
            DeviceMapperGuiManager.Instance.ListAllSingleInputConections();
            DeviceMapperGuiManager.Instance.ListAllSelectedGameobjectProperties();
        }

        /// <summary>
        /// Routes to the <see cref="MessageManager"/> instance to toggle the console window.
        /// </summary>
        public void ToggleConsoleWindow()
        {
            MessageManager.Instance.ToggleConsoleWindow();
        }

        /// <summary>
        /// Opens a confirmation window (<see cref="ConfirmationActionManager.ShowConfirmationWindow"/>) and resets teh calibration if the user confirms it.
        /// </summary>
        public void ResetCalibrationSettings()
        {
            ConfirmationActionManager.Instance.ShowConfirmationWindow(
                MapperManager.ResetAllDeviceConnections,
                "RESET CONFIGURATION");
        }

        /// <summary>
        /// Shows in the gui information for the selected device
        /// <para>Triggered when a device name in the Top menu of the GUI is clicked</para>
        /// </summary>
        /// <param name="go"></param>
        public void SelectDevice(GameObject go)
        {
            GenericDeviceGuiManager.Instance.ShowDeviceInformationPanel(go.name);
        }

        /// <summary>
        /// Shows in the gui the Selected Gameobject and its <see cref="GameObjectProperty"/>
        /// <para>Triggered when a Available Gameobject name in the right side menu of the GUI is clicked</para>
        /// </summary>
        /// <param name="go"></param>
        public void SelectGameObject(GameObject go)
        {
            DeviceMapperGuiManager.Instance.PopulateSelectedGameobjectWindow(go);
        }

        /// <summary>
        /// Shows the Edit/Add Property Window in the GUI
        /// <para>Triggered when the '+' button below the selected object window in  the GUI is clicked</para>
        /// </summary>
        public void ShowAddPropertyWindow()
        {
            DeviceMapperGuiManager.Instance.ShowAddPropertyWindow();
        }

        /// <summary>
        /// Adds or removes a Interpreter component to the <see cref="GameObjectProperty.Target"/> according to being active or not in the gui
        /// <para>Triggered when a button in the edit property window is clicked</para>
        /// </summary>
        public void UpdatePropertyAcivation(Toggle propToggle)
        {
            var infoType = Parser.StringToEnum<InformationType>(propToggle.GetComponentInChildren<Text>().text.ToLower());
            
            if (propToggle.isOn)
                DeviceMapperGuiManager.Instance.AddGameobjectProperty(DeviceMapperGuiManager.Instance.CurrentSelectedObject, infoType);
            else
                DeviceMapperGuiManager.Instance.RemoveGameobjectProperty(DeviceMapperGuiManager.Instance.CurrentSelectedObject, infoType);
        }

        /// <summary>
        /// Shows in the GUI the Editor Window for the specified <see cref="GameObjectProperty"/> type
        /// <para>Triggered when the calibrate button of the main position property is clicked</para>
        /// </summary>
        /// <param name="go">The <see cref="GameObjectPropertyGui"/> gameobject pressed</param>
        public void ShowPropertyEditorWindow(GameObject go)
        {
            var gameObjectPropertyGui = go.GetComponent<GameObjectPropertyGui>();

            PropertyEditorGuiManager.Instance.ShowPropertyEditorWindow(gameObjectPropertyGui);
        }

        /// <summary>
        /// Saves all the changes on the specified <see cref="GameObjectProperty"/> type of the current Selected Gameobject. Hides the window after.
        /// <para>Triggered when the Save button of the Property Editor window is clicked</para>
        /// </summary>
        public void HidePropertyEditorWindow()
        {
            if (InputCalibrator.Instance.Calibrating) return;
            PropertyEditorGuiManager.Instance.HidePropertyEditorWindow();
        }
        
        /// <summary>
        /// Updates the Dropdown for the axis Mapped
        /// <para>Triggered when one of the editor property dropdowns are changed</para>
        /// </summary>
        /// <param name="axisMapped">The Dropdown that was changed</param>
        public void UpdateAxisMapping(Dropdown axisMapped)
        {
            var axisMappedName = axisMapped.options[axisMapped.value].text;

            //Debug.Log(axisMappedName);
            PropertyEditorGuiManager.Instance.UpdateAxisMapped(axisMappedName, axisMapped.name);
        }

        /// <summary>
        /// Updates the Dropdown for the axis Mapped
        /// <para>Triggered when one of the single editor property dropdowns are changed</para>
        /// </summary>
        /// <param name="axisMapped">The Dropdown that was changed</param>
        public void SingleAxisMappingChanged(Dropdown axisMapped)
        {
            var axisMappedName = axisMapped.options[axisMapped.value].text;

            PropertyEditorGuiManager.Instance.
                UpdateAxisMappedCalibrationBtn(axisMappedName, PropertyEditorGuiManager.Instance.SingleEditorCalibrateBtn); 
        }

        /// <summary>
        /// Updates the Dropdown for the value Mapped
        /// <para>Triggered when one of the value editor property dropdowns are changed</para>
        /// </summary>
        /// <param name="axisMapped">The Dropdown that was changed</param>
        public void ValueMappingChanged(Dropdown axisMapped)
        {
            var axisMappedName = axisMapped.options[axisMapped.value].text;

            PropertyEditorGuiManager.Instance.UpdateSingleInputMapping(0, axisMappedName);

            PropertyEditorGuiManager.Instance.
                UpdateAxisMappedCalibrationBtn(axisMappedName, PropertyEditorGuiManager.Instance.ValueCalibrateBtn);
        }

        /// <summary>
        /// Updates the boolean editor window according to the selected dropdown label
        /// <para>Triggered when one of the boolean editor property dropdowns are changed</para>
        /// </summary>
        /// <param name="booleanDrop"></param>
        public void UpdateBooleanEditorDropdown(Dropdown booleanDrop)
        {
            var axisMappedName = booleanDrop.options[booleanDrop.value].text;
            
            PropertyEditorGuiManager.Instance.UpdateSingleInputMapping(0, axisMappedName);

            PropertyEditorGuiManager.Instance.
                UpdateAxisMappedCalibrationBtn(axisMappedName, PropertyEditorGuiManager.Instance.BooleanCalibrateBtn);
        }

        /// <summary>
        /// Updates the calibration mode 
        /// <para>Triggered when the use converter toggle is changed</para>
        /// </summary>
        /// <param name="useConverter">The toggle that was changed</param>
        public void UpdateUseDirectMode(Toggle useConverter)
        {
            PropertyEditorGuiManager.Instance.UpdateCalibrationDirectMode(useConverter);
        }

        /// <summary>
        /// Updates the calibration mode  to additive
        /// <para>Triggered when the use converter toggle is changed</para>
        /// </summary>
        /// <param name="joystickToggle">The toggle that was changed</param>
        public void UpdateUseAdditiveMode(Toggle joystickToggle)
        {
            PropertyEditorGuiManager.Instance.UpdateCalibrationToAdditiveMode(joystickToggle);
        }

        /// <summary>
        /// Updates if the data should be usedor not in the selected gameobject
        /// <para>Triggered when the "use on this obejct" toggle is changed</para>
        /// </summary>
        /// <param name="useOnThisObjectToggle">The toggle that was changed</param>
        public void UpdateUseOnThisObject(Toggle useOnThisObjectToggle)
        {
            PropertyEditorGuiManager.Instance.UpdateUseOnThisObject(useOnThisObjectToggle);
        }

        /// <summary>
        /// Updates the invert logic option in the calibration values according to the toggle state
        /// <para>Triggered when a Inver Logic Toggle is clicked</para>
        /// </summary>
        /// <param name="invertLogicToggle">The toggle that was changed</param>
        public void UpdateInverLogic(Toggle invertLogicToggle)
        {
            PropertyEditorGuiManager.Instance.UpdateInvertLogic(invertLogicToggle);
        }
    }
}
