using UnityEngine;
using UnityEngine.Events;

public class PrimaryRadar : RadarBase
{
    // DetectedObject, DirectionToTarget, DistanceToTarget
    public UnityEvent<GameObject, Vector2, float> OnObjectDetectedEvent = new UnityEvent<GameObject, Vector2, float>();
    
    protected override void OnTargetDetected(GameObject target)
    {
        // Calculate distance and 
        Vector2 targetPositionOnPlane = new Vector2(target.transform.position.x, target.transform.position.z);
        Vector2 radarPositionOnPlane = new Vector2(transform.position.x, transform.position.z);
        Vector2 directionToTarget = (targetPositionOnPlane - radarPositionOnPlane).normalized;
        
        float distance = (radarPositionOnPlane - targetPositionOnPlane).magnitude;
        
        OnObjectDetectedEvent.Invoke(target, directionToTarget, distance);
    }
}
