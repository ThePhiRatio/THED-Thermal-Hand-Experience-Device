using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FireTargetScript : MonoBehaviour
{
    //public AudioClip yay;
    public float sec;
    public Value value;

    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<AudioSource>().playOnAwake = false;
        // yay = GetComponent<AudioSource>().clip;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Fireball collided with Something");
        if (collision.transform.tag == "Fireball")
        {
            Debug.Log("Fireball collided with Firetarg");
            //GetComponent<AudioSource>().Play();
            value.setCurr_Fire(value.getCurr_Fire() + 1);
            StartCoroutine(waitsec());
        }
    }

    IEnumerator waitsec()
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }
}
