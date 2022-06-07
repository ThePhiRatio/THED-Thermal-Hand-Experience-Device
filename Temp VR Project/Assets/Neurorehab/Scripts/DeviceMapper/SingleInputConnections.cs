using System.Collections.Generic;

namespace Neurorehab.Scripts.DeviceMapper
{
    /// <summary>
    /// Responsible for having a connection between a <see cref="SingleInput"/> and all its <see cref="GameObjectProperty"/>
    /// </summary>
    public class SingleInputConnections
    {
        /// <summary>
        /// The <see cref="SingleInput"/> that has all the <see cref="ObjectProperties"/>
        /// </summary>
        public SingleInput SingleInput { get; set; }

        /// <summary>
        /// A list of <see cref="GameObjectProperty"/> belonging to the <see cref="SingleInput"/>
        /// </summary>
        public List<GameObjectProperty> ObjectProperties { get; set; }

        public SingleInputConnections(SingleInput singleInput, List<GameObjectProperty> list)
        {
            SingleInput = singleInput;
            ObjectProperties = list;
        }
    }
}
