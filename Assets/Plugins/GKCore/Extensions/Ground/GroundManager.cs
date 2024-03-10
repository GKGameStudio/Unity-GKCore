using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : SingletonMonoBehaviour<GroundManager>
{
    public LayerMask groundLayerMask;
    public bool IsGround(GameObject obj){
        return GKUtils.IsInLayerMask(obj.layer, groundLayerMask);
    }
}
