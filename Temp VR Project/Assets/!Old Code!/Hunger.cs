using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hunger : MonoBehaviour
{

    public float CurrHunger { get; set; }
    public float MaxHunger { get; set; }
    public float hungerSpeed;
    public Text status;
    public Slider hungerbar;
    public float startHuSpeed;
    private bool pressed;

    // Use this for initialization
    void Awake()
    {
        MaxHunger = 100f;
        CurrHunger = GetComponent<GameControl>().hunger;
        startHuSpeed = hungerSpeed;
        pressed = false;
    }

    void reset()
    {
        pressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrHunger <= 0)
        {
            SceneManager.LoadScene(8);
        }
        hungerbar.value = HungerCalc();
        status.text = "Hunger: " + CurrHunger.ToString();
        CurrHunger -= Time.fixedDeltaTime * hungerSpeed;
    }

    void OnCollisionStay(Collision coll)
    {
        if (pressed)
            return;

        if (coll.transform.tag == "Food")
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                pressed = true;
                if ((CurrHunger + coll.gameObject.GetComponent<HungerValue>().HungerVal) <= MaxHunger) CurrHunger = CurrHunger + coll.gameObject.GetComponent<HungerValue>().HungerVal;
                else CurrHunger = MaxHunger;
                Invoke("reset", 2f);
            }
        }
    }

    float HungerCalc()
    {
        return CurrHunger / MaxHunger;
    }
}
