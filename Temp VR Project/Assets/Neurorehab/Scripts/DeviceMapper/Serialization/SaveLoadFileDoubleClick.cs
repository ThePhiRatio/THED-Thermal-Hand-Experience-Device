using Neurorehab.Scripts.Utilities;
using Neurorehab.SimpleFileBrowser.Scripts.GracesGames;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Serialization
{
    /// <summary>
    /// Implements the <see cref="IDoubleClickAction"/> to add a double click action to the <see cref="FileBrowser"/>.
    /// </summary>
    public class SaveLoadFileDoubleClick : MonoBehaviour, IDoubleClickAction
    {
        /// <summary>
        /// Action performed once the double click the performed. Works both on save and load file.
        /// </summary>
        public void PerformDoubleClickAction()
        {
            FileBrowser.Instance.SelectFile();
        }
    }
}