using System.Collections.Generic;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for holding all the information of its own Gameobject, including all its <see cref="InformationType"/>
    /// </summary>
    public class GameObjectInformation : MonoBehaviour
    {
        /// <summary>
        /// The Target Gameobject wich will be mapped (who has all the Interpreter Components)
        /// </summary>
        public GameObject Target { get; set; }
        /// <summary>
        /// A list of all the <see cref="InformationType"/> that the gameobject has. Used to instatiate all the ObjectProperties
        /// </summary>
        public List<InformationType> GameObjectInformationTypes { get; set; }

        /// <summary>
        /// List of all the GameObjectProperties that the gameobject has.
        /// </summary>
        public List<GameObjectProperty> ObjectProperties { get; set; }

        /// <summary>
        /// Initializes all properties for the specific Target
        /// </summary>
        public void InitProperties()
        {
            //adiciona a lista de GameObjectInformationTypes todos os enums
            GameObjectInformationTypes = new List<InformationType>
            {
                InformationType.@bool,
                InformationType.position,
                InformationType.rotation,
                InformationType.value,
                InformationType.sample
            };
            

            //poe todos desactivados
            ObjectProperties = new List<GameObjectProperty>();

            foreach (var informationType in GameObjectInformationTypes)
            {
                ObjectProperties.Add(new GameObjectProperty
                {
                    InfoType = informationType,
                    Target = Target,
                    Active = false
                });
            }
        }
    }
}
