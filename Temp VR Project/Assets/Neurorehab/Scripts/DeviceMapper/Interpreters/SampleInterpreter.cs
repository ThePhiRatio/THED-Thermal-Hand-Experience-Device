using System.Collections;
using System.Collections.Generic;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Interpreters
{
    /// <summary>
    /// Responsible for mapping the <see cref="_gameObjectProperty"/> Sample. Currently UNTESTED
    /// </summary>
    public class SampleInterpreter : Interpreter
    {
        protected override void SetMappedValue()
        {
            Value.Sample = GameObjectProperty.GetSample();

            var sampleString = "";

            foreach (var value in Value.Sample)
            {
                sampleString += ", " + value;
            }

            print(sampleString);
        }
        

        protected override IEnumerator WaitingForCalibration(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            yield break;
        }

        protected override void ClampValue(MapperValue value)
        {
        }
    }
}