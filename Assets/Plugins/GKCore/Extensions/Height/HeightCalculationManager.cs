using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightCalculationManager : SingletonMonoBehaviour<HeightCalculationManager>
{
    public float GetHeightFromGround(GameObject obj){
        return GetHeightFromGround(obj.transform);
    }
    public float GetHeightFromGround(Transform trans){
        return GetHeightFromGround(trans.position);
    }
    public float GetHeightFromGround(Vector3 pos){
        //Using raycast to get the height from ground
        RaycastHit hit;
        if (Physics.Raycast(pos, -Vector3.up, out hit, 1000, GroundManager.instance.groundLayerMask)){
            return hit.distance;
        }
        return 0f;
    }
}
