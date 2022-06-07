using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class thedv2serialporthandler : MonoBehaviour
{
    // Singleton
    private static thedv2serialporthandler s_thedv2serialPortHandler = null;
    public static thedv2serialporthandler GetHandler() { return s_thedv2serialPortHandler; }


    // The serial port
    private SerialPort serialPort = new SerialPort("\\\\.\\COM13");

    // Start is called before the first frame update
    void Start()
    {
        // Singleton setup
        if (s_thedv2serialPortHandler != null)
        {
            Debug.LogError("Can't have more than one serial port handler.");
            Destroy(this.gameObject);
            return;
        }
        s_thedv2serialPortHandler = this;

        // Setup serial port
        Debug.Log("Setting up com port.");
        serialPort.PortName = "COM5"; // SET CORRECT COM PORT. IMPORTANT
        serialPort.DataBits = 8;  //was 8 but I think it needs to be 12
        serialPort.Parity = Parity.None;
        serialPort.StopBits = StopBits.One;
        serialPort.BaudRate = 115200;
        serialPort.Encoding = System.Text.Encoding.GetEncoding(28591);
        serialPort.Open();
        serialPort.DiscardOutBuffer();
        Debug.Log("Com port setup");
    }

    // Cleanup
    void OnDestroy()
    {
        if (serialPort != null)
        {
            Send(false, (char)0, (char)0, (char)2, (char)2 );
            //Send(true , (char)0, (char)0, (char)2, (char)2 );
            serialPort.Close();
            serialPort = null;
        }
    }

    // Send a message to the client
    public void Send(bool isLeftHand, char intensity_left, char intensity_right, char isHot_left, char isHot_right)
    {

        // Build Message
        char[] arr = new char[6];
        arr[0] = (char)254; // Need start character
        arr[1] = (char)(isLeftHand ? 1 : 0);
        arr[2] = (char)intensity_left;
        arr[3] = (char)intensity_right;      
        arr[4] = (char)isHot_left;
        arr[5] = (char)isHot_right;

        serialPort.Write(arr, 0, 6);
    }
}
