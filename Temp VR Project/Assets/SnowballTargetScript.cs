using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SnowballTargetScript : MonoBehaviour
{
    //public AudioClip yay;
    public float sec;
    public Value value;

    // Start is called before the first frame update
    void Start()
    {
       
       // GetComponent<AudioSource>().playOnAwake = false;
        //yay = GetComponent<AudioSource>().clip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Snowball collided with Something");
        if (collision.transform.tag == "Snowball")
        {
            Debug.Log("Snowball collided with Snowtarg");
            // GetComponent<AudioSource>().Play();
            value.setCurr_Snow(value.getCurr_Snow() + 1);
            StartCoroutine(waitsec());
        }
    }

    IEnumerator waitsec()
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }
}
