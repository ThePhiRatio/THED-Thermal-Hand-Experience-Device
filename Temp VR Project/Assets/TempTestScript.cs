using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTestScript : MonoBehaviour
{
    private char temp = (char)0;
    private char isHot = (char)2;
    float MAX_TIME = 1.0f;
 //   float DUTY_CYCLE = 1.0f;

    // TMP
    float time = 0;
    void Update(){
        
        time += Time.deltaTime;
        SerialPortHandler sph = SerialPortHandler.GetHandler();
        if(time >= MAX_TIME){
            time = 0.0f;
            /*
            DUTY_CYCLE = DUTY_CYCLE * 0.9f;
            if(DUTY_CYCLE < 0.25f)
                DUTY_CYCLE = 0.25f;
            */
            print((int)temp);
            print((int)isHot);
            sph.Send(false,temp,isHot);
            sph.Send(true,temp,isHot);
        } 
        /* 
        else if(time >= (MAX_TIME*DUTY_CYCLE)){
            sph.Send(false,(char)127,(char)127);
            sph.Send(true,(char)127,(char)127);
        } else {
            sph.Send(false,temp,temp);
            sph.Send(true,temp,temp);
        }
        */
        
    }


    public void SetTemp(int temp){
        this.temp = (char)temp;
     //   DUTY_CYCLE = 1.0f;
    }

    public void SetisHot(int isHot) {
        this.isHot = (char)isHot;
    }


}
