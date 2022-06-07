using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.CpDebugger.Gui
{
    /// <summary>
    /// Responsible for the mouse interaction (clicks) in the CPDebugger GUI. Added to any toggle prefab existent in the scene.
    /// </summary>
    public class CpDebuggerGuiClickManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="CpDebuggerGuiManager"/> component
        /// </summary>
        private CpDebuggerGuiManager _guiManager;

        /// <summary>
        /// The gameobject Toggle
        /// </summary>
        private Toggle _toggle;

        private void Start()
        {
            _guiManager = CpDebuggerGuiManager.Instance;
            _toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// Triggered when a device button is clicked (Kinect, Bitalino, etc)
        /// <para>If <see cref="_toggle"/> is On, Populates the Device Ids column. Otherwise, Unselects the device and clears all columns</para>
        /// </summary>
        /// <param name="btn">The Text value of the button clicked. Used to identify the Device clicked</param>
        public void ShowIds(Text btn)
        {
            if (_toggle.isOn)
            {
                var device = btn.text;
                _guiManager.PopulateIds(device);
            }
            else
            {
                _guiManager.UnselectOptions(DebuggerColumns.device);
            }
        }

        /// <summary>
        /// Triggered when a Id button is clicked (The device data identifier, a device can have more than one,for example each avatar detected by the kinect has a different id).
        /// <para>If <see cref="_toggle"/> is On, Populates the Device Categories column. Otherwise, Unselects the Id and clears all columns except device and ids</para>
        /// </summary>
        /// <param name="btn">The Text value of the button clicked. Used to identify the Id clicked</param>
        public void ShowCategories(Text btn)
        {
            if (_toggle.isOn)
            {
                var id = btn.text;
                _guiManager.PopulateCategories(id);
            }
            else
            {
                _guiManager.UnselectOptions(DebuggerColumns.id);
            }
        }

        /// <summary>
        /// Triggered when a Category button is clicked (tracking, button, digital, etc).
        /// <para>If <see cref="_toggle"/> is On, Populates the Device Labels column. Otherwise, Unselects the Category and clears all columns except device, ids and categories</para>
        /// </summary>
        /// <param name="btn">The Text value of the button clicked. Used to identify the Category clicked</param>
        public void ShowLabels(Text btn)
        {
            if (_toggle.isOn)
            {
                var category = btn.text;
                _guiManager.PopulateLabels(category);
            }
            else
            {
                _guiManager.UnselectOptions(DebuggerColumns.category);
            }
        }

        /// <summary>
        /// Triggered when a Label button is clicked (waist, d1, palm, etc).
        /// <para>If <see cref="_toggle"/> is On, Populates the Device Types column. Otherwise, Unselects the Label and clears all columns except devices, ids, categories and labels</para>
        /// </summary>
        /// <param name="btn">The Text value of the button clicked. Used to identify the Label clicked</param>
        public void ShowTypes(Text btn)
        {
            if (_toggle.isOn)
            {
                var label = btn.text;
                _guiManager.PopulateType(label);
            }
            else
            {
                _guiManager.UnselectOptions(DebuggerColumns.label);
            }
        }

        /// <summary>
        /// Triggered when a Type button is clicked (position, rotation, value etc).
        /// <para>If <see cref="_toggle"/> is On, Populates the Device Values and Parameters. Otherwise, unselects the Type and clears all columns except devices, ids, categories, labels and types</para>
        /// </summary>
        /// <param name="btn">The Text value of the button clicked. Used to identify the Type clicked</param>
        public void ShowValues(Text btn)
        {
            if (_toggle.isOn)
            {
                var type = btn.text;
                _guiManager.PopulateValues(type);
            }
            else
            {
                _guiManager.UnselectOptions(DebuggerColumns.type);
            }
        }
    }
}