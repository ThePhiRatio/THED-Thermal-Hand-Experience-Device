using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Value : MonoBehaviour
{

    public int curr_fire;// { get; set; }
    public int max_fire;
    public Text status_f;
    public Slider firebar;

    public int curr_snow; //{ get; set; }
    public int max_snow;
    public Text status_s;
    public Slider snowbar;

    public GameObject win_text;
    public GameObject fireworks;

    // Start is called before the first frame update
    void Start()
    {
        firebar.maxValue = 1;
        firebar.minValue = 0;
        snowbar.maxValue = 1;
        snowbar.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        firebar.value = (float)curr_fire / (float)max_fire;
        status_f.text = "Fire Targets: " + curr_fire + "/" + max_fire;

        snowbar.value = (float)curr_snow / (float)max_snow;
        status_s.text = "Snow Targets: " + curr_snow + "/" + max_snow;

        Debug.Log("Curr_Snow = " + curr_snow);
        Debug.Log("Curr_Fire = " + curr_fire);

        if(curr_fire == max_fire && curr_snow == max_snow)
        {
            Debug.Log("The game is finished.");
            win_text.SetActive(true);
            fireworks.SetActive(true);
        }
   
    }
    public int getCurr_Fire()
    {
        return curr_fire;
    }

    public int getCurr_Snow()
    {
        return curr_snow;
    }

    public void setCurr_Fire(int val)
    {
        curr_fire = val;
    }

    public void setCurr_Snow(int val)
    {
        curr_snow = val;
    }

}



