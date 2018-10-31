using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : CombatStats {

    //[SerializeField] protected BehaviourBase[] behaviourList; // i try to made this efficiently in global terms, but i dont found the way
    [SerializeField] protected GameObject[] DropList;
    [SerializeField] protected float[] DropProb;
    [SerializeField] protected NavMeshAgent nav;
    [SerializeField] protected bool staticEnemy;
    [SerializeField] protected Watch watch;

    [SerializeField] protected float detectionAngle;
    //protected Rigidbody rigidbody;
    private float dropOffsetZ = 0.25f;
    private float dropOffsetY = 1f;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        //rigidbody = GetComponent<Rigidbody>();

        if (DropList.Length != DropProb.Length)
        {
            Debug.LogError("DropList and DropProb don´t match in size");
        }
        watch = GetComponent<Watch>();
    }

    protected bool FaceAndCheckObjective(Vector3 targetPos, float stoppingPrecision) //Estos 2 metodos son para girar el enemigo y hacer que mire hace cualquier sitio
        //el primero comprueba si el enemigo esta dentro del rango de vision, el segundo simplemente este objeto hacia el objetivo
    {
        Quaternion angle = CaculateTargetRotation(targetPos);
        if (Mathf.Abs(Quaternion.Angle(angle, transform.rotation)) <= Mathf.Abs(stoppingPrecision))
        {
            return true; //This is an end condition
        }
        else return false;
    }

    protected void FaceObjective(Vector3 targetPos) 
    {
        float step = nav.angularSpeed * Time.deltaTime;
        Quaternion angle = CaculateTargetRotation(targetPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, step);
    }

    private Quaternion CaculateTargetRotation(Vector3 targetPos)
    {

        float angle = Mathf.Atan2(targetPos.x - transform.position.x, targetPos.z - transform.position.z);

        angle = 180 * angle / Mathf.PI;
        Vector3 vector3 = new Vector3(0, angle, 0);
        return Quaternion.Euler(vector3);
    }

    protected void DropItem()
    {
        float valueSum = 0;
        float randomValue = Random.value * 100f;
        for (int i = 0; i < DropProb.Length; i++)
        {
            valueSum += DropProb[i];
            if (valueSum >= randomValue)
            {
                GameObject spawnThis = Instantiate(DropList[i], transform.position, transform.rotation);
                spawnThis.transform.position += spawnThis.transform.TransformVector(new Vector3(0, dropOffsetY / spawnThis.transform.lossyScale.y, dropOffsetZ / spawnThis.transform.lossyScale.z));
                break;
            }
        }
    }

    public bool TestPlayerOnVisual()
    {
        GameObject playerFound = null;
        RaycastHit[] raycastHit = Physics.SphereCastAll(transform.position, gameObject.GetComponent<SphereCollider>().radius * gameObject.transform.lossyScale.x, Vector3.one);
        foreach (RaycastHit n in raycastHit)
        {
            if (n.collider.gameObject.name == "Player")
            {
                playerFound = n.collider.gameObject;
            }
        }
        if (playerFound == null)
        {
            return false;
        }
        else
        {
            return watch.FindPlayer(playerFound, detectionAngle);
        }
    }
    
}
