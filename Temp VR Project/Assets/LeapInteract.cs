using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class LeapInteract : MonoBehaviour
{
    public float GRAB_DIS = 0.1f;   //distance between fingers to grab
    public float RAD_SEARCH = 0.1f; //distance from object to grab
    public float SPEED = 12;        //throw speed
    private bool isGrabed = false;
    private GameObject snowball = null;
    public float sec = 2;           //amount of seconds for snowball to melt


    // A variable by Avery to handle ice melting
    private float lastMeltUpdate = 0;
    // A variable by Avery to handle the initial snowball scale
    private Vector3 initialScale = Vector3.zero;


    //Update called once per second
    void Update()
    {
        Leap.Hand hand = this.GetComponent<Leap.Unity.CapsuleHand>().GetLeapHand();
        float distance = hand.Fingers[0].TipPosition.DistanceTo(hand.Fingers[2].TipPosition);
        Vector3 offset = hand.PalmNormal.ToVector3() * 0.035f;

        //if we aren't grabbing and want to, try to grab
        if (!isGrabed && (distance < GRAB_DIS))
        {
            Collider[] returnedObjects = Physics.OverlapSphere(
                /* center of search area (palm) */ (hand.PalmPosition.ToVector3() + offset),
                /* radius of search */ RAD_SEARCH,
                /* What layer to user */ LayerMask.GetMask("Snowball"),
                /* Interact with triggers? (optional) */ QueryTriggerInteraction.Ignore
                );
            float min = float.MaxValue;
            int index = -1;
            
            //iterate through the nearby objects and pick up only the closest one
            for (int i = 0; i < returnedObjects.Length; i++)
            {
                float curr = Vector3.Distance((hand.PalmPosition.ToVector3() + offset), returnedObjects[i].gameObject.transform.position);
                if (min > curr)
                {
                    min = curr;
                    index = i;
                }
            }
            if (index != -1)
            {
                snowball = returnedObjects[index].gameObject;
                isGrabed = true;
                returnedObjects[index].transform.position = (hand.PalmPosition.ToVector3() + offset);
                Rigidbody r = returnedObjects[index].GetComponent<Rigidbody>();
                if (r)
                    r.velocity = Vector3.zero;

                // Code by Avery to handle melting.
                // When we grab a snowball, it takes time to melt.
                // We need to store how big the snowball was when
                // it was first grabbed. We store this in a variable
                // called 'initialScale'
                initialScale = snowball.transform.localScale;

                // We also need to reset the timer for when the snowball
                // will fully melt.
                lastMeltUpdate = 0;
                // This DOES NOT account for a snowball that is halfway
                // melted. A halfway melted snowball will still take
                // the full amount of time to melt fully.
                // THIS IS A HACK. It SHOULD be moved to a Snowball
                // class that keeps track of the lifespan for each
                // individual snowball. (Definitly a Monobehaviour)
                // THIS WORKS FOR NOW, but isn't the best option.

            }
        }

        //if we are grabbing and we want to throw
        else if (isGrabed && (distance > GRAB_DIS))
        {
            snowball.transform.position = (hand.PalmPosition.ToVector3() + offset);
            Rigidbody r = snowball.GetComponent<Rigidbody>();
            if (r)
            {
                r.velocity = hand.PalmNormal.ToVector3() * SPEED;
            }
            snowball = null;
            isGrabed = false;
        }

        //if we are grabbing and we don't want to throw
        else if(isGrabed)
        {
            snowball.transform.position = (hand.PalmPosition.ToVector3() + offset);
            Rigidbody r = snowball.GetComponent<Rigidbody>();
            if (r)
                r.velocity = Vector3.zero;
            
            /* Nick's code
            //'melt' snowball over time in hand decrease scale 100 times over 2 sec, then destroy object
            for (int i = 0; i < 100; i++)
            {
                Vector3 scale = snowball.transform.localScale;
                scale *= (100 - (i * 100));
                snowball.transform.localScale = scale;
                if (i == 99)
                {
                    Destroy(snowball);
                    isGrabed = false;
                }
                StartCoroutine(Wait((float)(sec / 100)));
            }
            */


            // How Avery would go about implementing the snowball
            // Hopefully step by step enough to explain my point
            // First, we need to keep track of time.
            // We do this by creating a class variable that
            // stores the last time we updated the snowball.
            // This variable is defined far above and called 'lastMeltUpdate'.
            // Every frame, when an object is grabbed, we need to increase this
            // based on the time it took unity to run last frame.
            lastMeltUpdate += Time.deltaTime;

            // Check to see if the snowball is fully melted.
            // This uses the 'sec' variable, which defines how long it takes
            // for the snowball to fully melt.
            if(lastMeltUpdate > sec){

                // If the snowball is melted, stop grabbing it
                Destroy(snowball);
                isGrabed = false; // Grabbed has 2 'B's

            // If it hasn't melted yet, shrink the snowball according to
            // the amount of time that has passed
            } else {

                // Calculate the percent of the snowballs
                // lifetime that occured last frame
                float percentOfSnowballLife = Time.deltaTime / sec;

                // Calculate the amount to decrease the snowball scale.
                // This is based on the initial snowball size when it was
                // grabbed.
                Vector3 scaleReductionAmount = 
                            percentOfSnowballLife *
                            initialScale;

                // Update the snowball scale based on the
                // reduction amount calculated.
                snowball.transform.localScale =
                    snowball.transform.localScale - scaleReductionAmount;


            }


        }

        
    }

    public bool getGrabbed()
    {
        return isGrabed;
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
}
