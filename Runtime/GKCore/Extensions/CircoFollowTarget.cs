using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircoFollowTarget : MonoBehaviour
{
    public Transform target;
    public Rigidbody projectileRigidbody;

    public float projectileVelocity;
    public float projectileTurningRate;

    private bool following = true;

    
    

    //Update transform rotation to look at target smoothly
    public void CircoStartFollowingTarget(Transform target, Rigidbody projectileRigidbody, float projectileVelocity, float projectileTurningRate, float delay = 0, float followingPeriod = -1){
        GKUtils.RunAfterSeconds(()=>{
            this.target = target;
            this.projectileRigidbody = projectileRigidbody;
            this.projectileVelocity = projectileVelocity;
            this.projectileTurningRate = projectileTurningRate;

            if(followingPeriod > 0){
                GKUtils.RunAfterSeconds(()=>{
                
                this.following = false;
                
                }, followingPeriod);
            }
            
            
        }, delay);

        
        
    }
    private void UpdateMovement(){
        if(target != null && following){
            Vector3 targetPosition = target.transform.position;

            projectileRigidbody.velocity = transform.forward * projectileVelocity;

            var projectileTargetRotation = Quaternion.LookRotation(targetPosition - transform.position); 

            projectileRigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, projectileTargetRotation, projectileTurningRate));
        }
    }


    protected void FixedUpdate() {
        UpdateMovement();
        
    }
}
