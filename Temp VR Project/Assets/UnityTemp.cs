using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class UnityTemp : MonoBehaviour {

    public Transform target;    //transform of object that has heat or cold
    public bool isleft = false; //check to see if its the left or right hand
    private Leap.Hand hand;     //the hand object in question
    public LeapInteract interaction;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
    const float DELAY_MAX = 0.25f;
    float currentDelay = 0;
	void Update () {

        // Only update every so often
        // Time is defined as 1 = 1 second
        currentDelay += Time.deltaTime;
        if(currentDelay > DELAY_MAX){
            currentDelay = 0;

            //get the hand for the current frame
            hand = this.GetComponent<Leap.Unity.CapsuleHand>().GetLeapHand();

            //get distance and rotation of hand from the target object
            float distance = GetDistance();
            float angle = GetRotation();

            //calculate distance, clamped between 0 and 255
            distance = ((1 - Mathf.Clamp((distance - 1), 0, 1)) * 127) + 127;

            //calculate percents of heat by rotation of hand
            float percent_front = 1 - (angle / 180);
            float percent_back = 1 - percent_front;

            //calculate heat value of hand for given frame
            float heat_front = Mathf.Clamp((distance) * percent_front,0,255);
            float heat_back = Mathf.Clamp((distance) * percent_back,0,255);


            // Distance is good, but if we make it cold (below 127) then things
            // begin to heat up too much.
            if(heat_front < 127)
                heat_front = 127;
            if(heat_back < 127)
                heat_back = 127;

            // If grabbing a snowball, set heat to zero
            if (interaction.getGrabbed())
                heat_front = 0;

            // Get the serial port handler
            SerialPortHandler s = SerialPortHandler.GetHandler();
            if(s != null){
                s.Send(isleft,(char)heat_front,(char)heat_back);
                Debug.Log(heat_front + " " + heat_back);
            }
        }
	}

    float GetDistance ()
    {
        //POST: gives the distance between the target object and the palm of the hand
        return Vector3.Distance(hand.PalmPosition.ToVector3(), target.position);
    }

    float GetRotation ()
    {
        //POST: gives the angle of that the palm of the hand is facing towards target object; 0 if facing, 180 if palm is facing
        return Vector3.Angle(target.position - hand.PalmPosition.ToVector3(), hand.PalmNormal.ToVector3());
    }

}
