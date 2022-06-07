using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance_Heat : MonoBehaviour
{
    public GameObject camera;
    private float heat_front;
    private bool isleft;
    private float heat_back = 0;
    private int isHot;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y, (float)291.472);
            camera.transform.position = newPos;
        isleft = false;
        heat_front = 127;
        isHot = 2;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y, (float)293.472);
            camera.transform.position = newPos;
            heat_front = 255;
            isHot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y, (float)292.472);
            camera.transform.position = newPos;
            heat_front = 180;
            isHot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y, (float)291.472);
            camera.transform.position = newPos;
            heat_front = 0;
            isHot = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y, (float)290.472);
            camera.transform.position = newPos;
            heat_front = 180;
            isHot = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y, (float)289.472);
            camera.transform.position = newPos;
            heat_front = 255;
            isHot = 0;
        }
        SerialPortHandler s = SerialPortHandler.GetHandler();
        if (s != null)
        {
            s.Send(isleft, (char)heat_front, (char)isHot);
            Debug.Log(heat_front + " " + isHot);
        }
        
    }

    public float getHeat()
    {
        return heat_front;
    }

    public float getisHot()
    {
        return isHot;
    }


}
