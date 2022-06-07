using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialPortHandler : MonoBehaviour
{
    // Singleton
    private static SerialPortHandler s_serialPortHandler = null;
    public static SerialPortHandler GetHandler(){return s_serialPortHandler;}


    // The serial port
    private SerialPort serialPort = new SerialPort("\\\\.\\COM13");

    // Start is called before the first frame update
    void Start() {
        // Singleton setup
        if(s_serialPortHandler != null){
            Debug.LogError("Can't have more than one serial port handler.");
            Destroy(this.gameObject);
            return;
        } 
        s_serialPortHandler = this;

        // Setup serial port
        Debug.Log("Setting up com port.");
        serialPort.PortName = "COM5"; // SET CORRECT COM PORT. IMPORTANT
        serialPort.DataBits = 8;
        serialPort.Parity = Parity.None;
        serialPort.StopBits = StopBits.One;
        serialPort.BaudRate = 115200;
        serialPort.Encoding = System.Text.Encoding.GetEncoding(28591);
        serialPort.Open();
        serialPort.DiscardOutBuffer();
        Debug.Log("Com port setup");
    }

    // Cleanup
    void OnDestroy(){
        if(serialPort != null){
            Send(false,(char)127,(char)0 /*(char)127 */);
            Send(true,(char)127,(char)0 /*(char)127 */);
            serialPort.Close();
            serialPort = null;
        }
    }

    // Send a message to the client
    public void Send(bool isLeftHand, char frontTemp, char isHot /*char backTemp*/){

        // Build Message
        char[] arr = new char[4];
        arr[0] = (char)254; // Need start character
        arr[1] = (char)(isLeftHand ? 1 : 0);
        arr[2] = (char)frontTemp;
        arr[3] = (char)isHot;      //arr[3] = (char)backTemp;

        serialPort.Write(arr,0,4);
    }

}
