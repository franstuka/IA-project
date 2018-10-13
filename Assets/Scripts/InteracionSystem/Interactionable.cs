using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactionable : MonoBehaviour {

    public float Radius;
    public float TransicionTime = 0.5f;
    public Transform InteractionTransform;

    [SerializeField] private SphereCollider InteractionableZone;

    private Interactionable interactionable;

    private void Awake()
    {
        CheckTransform();
        interactionable = GetComponent<Interactionable>();   
    }

    private void Start()
    {
        Radius = InteractionableZone.radius * transform.lossyScale.x;
    }


    private void OnDrawGizmosSelected()
    {
        CheckTransform();
        Gizmos.color = Color.Lerp(Gizmos.color, Color.white, TransicionTime);
        Gizmos.DrawWireSphere(InteractionTransform.position, Radius); 
    }

    public virtual void Interact()
    {
        Debug.Log("im usefull");
    }
    public virtual void Interact(Interactionable interactionable) 
    {
        Debug.Log("im usefull V2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player" && other.tag == "Player")
        {
            ItemsNear.instance.AddNewElement(interactionable);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.name == "Player" && other.tag == "Player")
        {
            ItemsNear.instance.DeleteElement(interactionable);
        }
    }

    public void CheckTransform()
    {
        if (InteractionTransform == null) //We can change the interacction point with this, or let it by default
        {
            InteractionTransform = GetComponent<Transform>();
        }
    }
}
