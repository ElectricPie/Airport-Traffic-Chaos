using UnityEngine;
using UnityEngine.Events;

public class PrimaryRadar : RadarBase
{
    public UnityEvent<Vector2> OnTargetDetectedEvent = new UnityEvent<Vector2>();
    
    protected override void OnTargetDetected(GameObject target)
    {
        Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.z);
        OnTargetDetectedEvent.Invoke(targetPosition);
    }
}
