using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour
{

    public float sec;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Terrain" || collision.transform.tag == "FireTarg" || collision.transform.tag == "SnowTarg")
        {
                StartCoroutine(waitsec());
        }
    }
    IEnumerator waitsec()
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }
}
