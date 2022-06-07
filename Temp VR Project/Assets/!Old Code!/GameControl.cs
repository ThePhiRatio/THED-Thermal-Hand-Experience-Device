using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

    public static GameControl control;

    public float health;
    public float thirst;
    public float hunger;
    private Vector3 player_pos;



    // Use this for initialization
    void Awake()
    {
        control = this;        
    }
    //void Awake () {
    //       if(control == null)
    //       {
    //           DontDestroyOnLoad(gameObject);
    //           control = this;
    //       }
    //       else if(control != this)
    //       {
    //           Destroy(gameObject);
    //       }	
    //}
    public void Update ()
    {
        player_pos = gameObject.transform.position;
        
    }

    public void Save()
    {
        thirst = GetComponent<Thirst>().CurrThirst;
        health = GetComponent<Health>().CurrHealth;
        hunger = GetComponent<Hunger>().CurrHunger;
        player_pos = gameObject.transform.position;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.health = health;
        data.hunger = hunger;
        data.thirst = thirst;
        data.rx = player_pos.x;
        data.ry = player_pos.y;
        data.rz = player_pos.z;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            health = data.health;
            hunger = data.hunger;
            thirst = data.thirst;
            player_pos.x = data.rx;
            player_pos.y = data.ry;
            player_pos.z = data.rz;

            GetComponent<Thirst>().CurrThirst = thirst;
            GetComponent<Health>().CurrHealth = health;
            GetComponent<Hunger>().CurrHunger= hunger;
            gameObject.transform.position = player_pos;
        }

    }
}

[Serializable]
class PlayerData
{
    public float rx;
    public float ry;
    public float rz;
    public float health;
    public float hunger;
    public float thirst;
}
