using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thedv2temptest : MonoBehaviour
{

    private char temp = (char)0;
    private char isHot = (char)2;
    float MAX_TIME = 1.0f;

    // TMP
    float time = 0;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        thedv2serialporthandler sph = thedv2serialporthandler.GetHandler();
        thedv2rightserialporthandler sph_r = thedv2rightserialporthandler.GetHandler();
        if (time >= MAX_TIME)
        {
            time = 0.0f;
           
            print((int)temp);
            print((int)isHot);
            sph_r.Send(false, temp, temp, isHot, isHot);
            sph.Send(false, temp, temp, isHot, isHot);
            //sph.Send(true, temp, temp, isHot, isHot);
        }
    }

    public void SetTemp(int temp)
    {
        this.temp = (char)temp;
    }

    public void SetisHot(int isHot)
    {
        this.isHot = (char)isHot;
    }

}
