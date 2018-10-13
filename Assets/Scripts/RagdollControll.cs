using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollControll : MonoBehaviour {

    [SerializeField] private List<Component> componentsToDestroy;
    [SerializeField] private GameObject ActiveBones;
    [SerializeField] private GameObject ActiveMesh;
    [SerializeField] private GameObject RagdollBones;
    [SerializeField] private GameObject RagdollMeshGroup;
    private bool active;

    public void ActivateRagdoll()
    {
        if (!active)
        {
            active = true;
            RagdollMeshGroup.SetActive(true);
            SyncronizeBonesPosition();
            while (componentsToDestroy.Count > 0)
            {
                Destroy(componentsToDestroy[0]);
                componentsToDestroy.RemoveAt(0);
            }
            ActiveBones.SetActive(false);
            ActiveMesh.SetActive(false);
        }
        
    }

    private void SyncronizeBonesPosition()
    {
        Transform[] RagdollBonesList = RagdollBones.GetComponentsInChildren<Transform>(); //ragdollBones is the root
        Transform[] ActiveMeshBonesList = ActiveBones.GetComponentsInChildren<Transform>();

        //IMPORTANT, THE HIERARCHY MUST BE THE SAME, WITH THE SAME NAMES AND STRUCTURES

        if(RagdollBonesList.Length != ActiveMeshBonesList.Length)
        {
            Debug.LogError("ERROR IN RAGDOLLCONTROL, HIERARCHY DON'T HAS SAME SIZE");
        }
        else
        {
           
            RagdollBones.transform.position = ActiveBones.transform.position; //Root case
            RagdollBones.transform.rotation = ActiveBones.transform.rotation;
            RagdollBones.transform.localScale = ActiveBones.transform.localScale;
            for (int i = 0; i< RagdollBonesList.Length; i++ )
            {
                RagdollBonesList[i] = ActiveMeshBonesList[i];         
            }
        }
    }

    public bool IsActive()
    {
        return active;
    }
}
