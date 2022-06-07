using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_script_game : MonoBehaviour
{

    public Valve.VR.InteractionSystem.Hand left_hand;
    public Valve.VR.InteractionSystem.Hand right_hand;

    float MAX_TIME = 1.0f;
    float time = 0;

    private char L_temp;
    private char R_temp;
    private char L_isHot;
    private char R_isHot;

    public int snow_temp;
    public int fire_temp;

    // Start is called before the first frame update
    void Start()
    {
        L_temp = (char)0;
        R_temp = (char)0;
        L_isHot = (char)2;
        R_isHot = (char)2;
    }
    // Update is called once per frame
    void Update()
    {
        //Sending data
        time += Time.deltaTime;
        thedv2serialporthandler sph = thedv2serialporthandler.GetHandler();
        thedv2rightserialporthandler sph_r = thedv2rightserialporthandler.GetHandler();
        if (time >= MAX_TIME)
        {
            time = 0.0f;

            Debug.Log("Stuff Being Sent: L_temp = " + L_temp + " R_temp = " + R_temp + " L_isHot = " + L_isHot + " R_isHot = " + R_isHot + ".");
            sph_r.Send(false, L_temp, R_temp, L_isHot, R_isHot);
            sph.Send(false, L_temp, R_temp, L_isHot, R_isHot);
        }


        //if left hand has something in it
        if (left_hand.currentAttachedObject != null)
        {
            Debug.Log("The Left Hand is holding something.");
            //if the left hand has a snowball in it
            if (left_hand.currentAttachedObject.CompareTag("Snowball"))
            {
                L_isHot = (char)0;
                L_temp = (char)snow_temp;
                Debug.Log("The left hand is holding a snowball.");
            }
            //else if the left hand has a fireball in it
            else if (left_hand.currentAttachedObject.CompareTag("Fireball"))
            {
                L_isHot = (char)1;
                L_temp = (char)fire_temp;
                Debug.Log("The left hand is holding a fireball");
            }
        }
        //else if the left hand does not have something in it
        else
        {
            Debug.Log("Nothing in Left hand.");
            L_temp = (char)0;
            L_isHot = (char)2;
        }



        //if right hand has something in it
        if (right_hand.currentAttachedObject != null)
        {
            Debug.Log("The Right Hand is holding something.");
            //if the right hand has a snowball in it
            if (right_hand.currentAttachedObject.CompareTag("Snowball"))
            {
                R_isHot = (char)0;
                R_temp = (char)snow_temp;
                Debug.Log("The right hand is holding a snowball.");
            }
            //if the right hand has a fireball in it
            if (right_hand.currentAttachedObject.CompareTag("Fireball"))
            {
                R_isHot = (char)1;
                R_temp = (char)fire_temp;
                Debug.Log("The left hand is holding a fireball.");
            }
        }
        //else if the right hand does not have something in it
        else
        {
            Debug.Log("Nothing in Right hand.");
            R_temp = (char)0;
            R_isHot = (char)2;
        }
    }
}
