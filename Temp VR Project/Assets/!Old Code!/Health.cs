using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Health : MonoBehaviour {

    public float CurrHealth { get; set; }
    public float MaxHealth { get; set; }
    public float healthSpeed;
    public Text status;
    public Slider healthbar;
    public float startHeSpeed;
    private bool pressed;

    // Use this for initialization
    void Awake () {
        MaxHealth = 100f;
        CurrHealth = GetComponent<GameControl>().health;
        startHeSpeed = healthSpeed;
        pressed = false;
    }

    void reset()
    {
        pressed = false;
    }

    // Update is called once per frame
    void Update () {
        if (CurrHealth <= 0)
        {
            SceneManager.LoadScene(8);
        }
        healthbar.value = HealthCalc();
        status.text = "Health: " + CurrHealth.ToString();
        CurrHealth += Time.fixedDeltaTime * healthSpeed;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.tag == "Hurt")
        {
            CurrHealth = CurrHealth - coll.gameObject.GetComponent<HealthValue>().HealthVal;
        }
    }

    void OnCollisionStay(Collision coll)
    {
        if (pressed)
            return;

        if (coll.transform.tag == "Health")
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                pressed = true;
                if ((CurrHealth + coll.gameObject.GetComponent<HealthValue>().HealthVal) <= MaxHealth) CurrHealth = CurrHealth + coll.gameObject.GetComponent<HealthValue>().HealthVal;
                else CurrHealth = MaxHealth;
                Invoke("reset", 2f);
            }
        }
        if (coll.transform.tag == "Enemy")
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                pressed = true;
                CurrHealth = CurrHealth - coll.gameObject.GetComponent<HealthValue>().HealthVal;
                Invoke("reset", 2f);
            }
        }
    }
    float HealthCalc()
    {
        return CurrHealth / MaxHealth;
    }
}
