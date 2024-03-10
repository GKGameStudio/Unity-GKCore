using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShadow : MonoBehaviour
{
    [Header("Ball shadow")]
    public GameObject ballObject;
    public GameObject ballShadow;
    public float ballShadowCurrentAlpha;
    public float ballShadowTargetAlpha = 0f;
    public Vector2 showShadowHeightRange = new Vector2(5, 1000000);
    public void UpdateBallShadowMechanics(){
        float height = HeightCalculationManager.instance.GetHeightFromGround(ballObject.transform.position);
        //Update position
        Vector3 ballShadowPosition = transform.position;
        ballShadowPosition.y -= height;
        if(ballShadowPosition.y == -Mathf.Infinity){
            ballShadowPosition.y = 0;
        }
        ballShadow.transform.position = ballShadowPosition;
        
        //Update alpha
        if(height > showShadowHeightRange.x && height < showShadowHeightRange.y){
            ballShadowTargetAlpha = 1f;
        }else{
            ballShadowTargetAlpha = 0f;
        }
        if(ballShadowCurrentAlpha != ballShadowTargetAlpha){
            ballShadowCurrentAlpha = Mathf.Lerp(ballShadowCurrentAlpha, ballShadowTargetAlpha, Time.deltaTime * 5f);
            //Find all SpriteRenderer in the ballShadow
            SpriteRenderer[] spriteRenderers = ballShadow.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer spriteRenderer in spriteRenderers){
                Color color = spriteRenderer.color;
                color.a = ballShadowCurrentAlpha;
                spriteRenderer.color = color;
            }
        }
        if(ballShadowCurrentAlpha > 0){
            ballShadow.transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 10f) * 0.1f);
        }
    }
    void Update()
    {
        UpdateBallShadowMechanics();
    }
}
