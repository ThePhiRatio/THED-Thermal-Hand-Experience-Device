using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Thirst : MonoBehaviour {

    public float CurrThirst { get; set; }
    public float MaxThirst { get; set; }
    public float thirstSpeed;
    public Text status;
    public Slider thirstbar;
    public float startTSpeed;
    private bool pressed;

    // Use this for initialization
    void Awake () {
        MaxThirst = 100f;
        CurrThirst = GetComponent<GameControl>().thirst;
        startTSpeed = thirstSpeed;
        pressed = false;
    }

    void reset()
    {
        pressed = false;
    }

    // Update is called once per frame
    void Update () {
        if (IsUnderwater())
        {
            if (pressed)
                return;

            if (Input.GetKeyDown(KeyCode.V))
            {
                pressed = true;
                if ((CurrThirst + 5) <= MaxThirst) CurrThirst = CurrThirst + 5;
                else CurrThirst = MaxThirst;
                Invoke("reset", 2f);
            }
        }
        if (CurrThirst <= 0)
        {
            SceneManager.LoadScene(8);
        }
        thirstbar.value = ThirstCalc();
        status.text = "Thirst: " + CurrThirst.ToString();
        CurrThirst -= Time.fixedDeltaTime * thirstSpeed;
       // GameControl.control.thirst = CurrThirst;
    }

    bool IsUnderwater ()
    {
        return this.gameObject.transform.position.y < 50.0f;
    } 

    //void OnCollisionStay(Collision coll)
    //{
    //    if (coll.transform.tag == "Drink")
    //    {
    //        if (Input.GetKeyDown(KeyCode.V))
    //        {
    //            CurrThirst = CurrThirst + coll.gameObject.GetComponent<ThirstValue>().ThirstVal;
    //        }
    //    }
    //}

    float ThirstCalc()
    {
        return CurrThirst / MaxThirst;
    }
}
