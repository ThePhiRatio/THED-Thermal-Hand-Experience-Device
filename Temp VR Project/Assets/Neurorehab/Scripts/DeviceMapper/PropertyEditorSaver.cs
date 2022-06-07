using System;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;

namespace Neurorehab.Scripts.DeviceMapper
{
    /// <summary>
    /// Responsible for saving the settings of each the Property Editor Window in the GUI
    /// </summary>
    public static class PropertyEditorSaver
    {
        /// <summary>
        /// Saves the max reading value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the max reading value will be saved on</param>
        public static void SaveMaxInputValue(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance. SelectedGameObjectPropertyGui.GameObjectProperty;
            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetMaxInputValue(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetMaxInputValue(value, axis);
                    break;
                case InformationType.@bool:
                    target.GetComponent<BooleanInterpreter>().SetMaxInputValue(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetMaxInputValue(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the min reading value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        public static void SaveMinInputValue(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetMinInputValue(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetMinInputValue(value, axis);
                    break;
                case InformationType.@bool:
                    target.GetComponent<BooleanInterpreter>().SetMinInputValue(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetMinInputValue(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the min world reading value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        public static void SaveMinOutputValue(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;
            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetMinOutputValue(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetMinOutputValue(value, axis);
                    break;
                case InformationType.@bool:
                    //target.GetComponent<BooleanInterpreter>().SetMinOutputValue(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetMinOutputValue(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the max world reading value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        /// 
        public static void SaveMaxOutputValue(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;
            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetMaxOutputValue(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetMaxOutputValue(value, axis);
                    break;
                case InformationType.@bool:
                    //target.GetComponent<BooleanInterpreter>().SetMaxOutputValue(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetMaxOutputValue(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the center reading value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        /// 
        public static void SaveCenterInputValue(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetCenterInputValue(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetCenterInputValue(value, axis);
                    break;
                case InformationType.@bool:
                    //target.GetComponent<BooleanInterpreter>().SetCenterInputValue(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetCenterInputValue(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Saves the calibration mode in the correct interpreter
        /// </summary>
        /// <param name="mode"></param>
        public static void SaveCalibrationMode(CalibrationMode mode)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().Mode = mode;
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().Mode = mode;
                    break;
                case InformationType.@bool:
                    //target.GetComponent<BooleanInterpreter>().SetCalibrationMode(mode);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().Mode = mode;
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the speed value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        /// 
        public static void SaveSpeed(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetSpeed(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetSpeed(value, axis);
                    break;
                case InformationType.@bool:
                    //target.GetComponent<BooleanInterpreter>().SetSpeed(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetSpeed(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the tolerance value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        /// 
        public static void SaveTolerance(float value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    currentGop.GetRotInterpreter().SetTolerance(value, axis);
                    break;
                case InformationType.position:
                    currentGop.GetPosInterpreter().SetTolerance(value, axis);
                    break;
                case InformationType.value:
                    currentGop.GetValueInterpreter().SetTolerance(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Saves the threshold value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        public static void SaveThresholdValue(float value)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            currentGop.GetBoolInterpreter().CalibrationValues.InputData[AxisLabels.Bool].Center = value;
        }

        /// <summary>
        /// Saves the tolerance value acording to the axis and in the correct interpreter
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min reading value will be saved on</param>
        /// 
        public static void SaveClampValue(bool value, AxisLabels axis)
        {
            var currentGop = PropertyEditorGuiManager.Instance.SelectedGameObjectPropertyGui.GameObjectProperty;

            var target = currentGop.Target;

            switch (currentGop.InfoType)
            {
                case InformationType.rotation:
                    target.GetComponent<RotationInterpreter>().SetClampValue(value, axis);
                    break;
                case InformationType.position:
                    target.GetComponent<PositionInterpreter>().SetClampValue(value, axis);
                    break;
                case InformationType.@bool:
                    //target.GetComponent<BooleanInterpreter>().SetClampValue(value, axis);
                    break;
                case InformationType.value:
                    target.GetComponent<ValueInterpreter>().SetClampValue(value, axis);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}