using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10f;
    public bool updateRotation = true;

    //Update transform rotation to look at target smoothly
    public void StartFollowingTarget(Transform target, float followSpeed, bool updateRotation, float delay = 0){
        GKUtils.RunAfterSeconds(()=>{
            this.target = target;
            this.followSpeed = followSpeed;
            this.updateRotation = updateRotation;
        }, delay);
    }
    private void UpdateMovement(){
        if(target != null){
            Vector3 targetPosition = target.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
    private void UpdateRotation(){
        if(target != null){
            transform.LookAt(target);
        }
    }


    protected void FixedUpdate() {
        UpdateMovement();
        UpdateRotation();
    }
}
