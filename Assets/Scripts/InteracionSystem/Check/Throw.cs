using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public int vel = 3;
    public GameObject stone;
    

	void Update () {

        if (Input.GetKeyDown("e"))
        {
            GameObject clone2 = Instantiate(stone, transform.position + new Vector3(1, 1, 0), transform.rotation);
            clone2.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * vel);
            Destroy(clone2, 5);
        }

    }
}
