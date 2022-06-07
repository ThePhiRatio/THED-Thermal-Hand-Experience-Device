using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveThing : MonoBehaviour
{

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                StartCoroutine(waitsec());
            }
        }
    }

    IEnumerator waitsec()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
