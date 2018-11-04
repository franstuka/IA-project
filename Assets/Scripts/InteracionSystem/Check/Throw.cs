using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public int vel = 3;
    public GameObject stone;
    private int waitingTime = 5;
    private bool canThrow = true;



    

	void Update () {

        if (Input.GetKeyDown("e") && canThrow)
        {
            GameObject clone = Instantiate(stone, transform.position, transform.rotation);
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * vel);
            Destroy(clone, waitingTime);
            canThrow = false;
            StartCoroutine(Recharge());
        }

    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(waitingTime);
        canThrow = true;
    }
}
