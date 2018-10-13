using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSite : MonoBehaviour {

    public bool active = true;
    private float waitTime = 5f;

	public Transform GetTransform()
    {
        return transform;
    }

    public bool IsActive()
    {
        return active;
    }

    IEnumerator WaitToActive()
    {
        yield return new WaitForSeconds(waitTime);
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            active = false;
            StopAllCoroutines();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(WaitToActive());
        }
    }

}
