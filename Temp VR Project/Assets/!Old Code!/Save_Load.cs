using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Load : MonoBehaviour {

    void OnGUI()
    {
        if(GUI.Button(new Rect(10,300,100,30), "Save"))
        {
            GameControl.control.Save();
        }

        if(GUI.Button(new Rect(10, 340, 100, 30), "Load"))
        {
            GameControl.control.Load();
        }
    }
}
