using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RadarEmitter : MonoBehaviour
{
    [Tooltip("The max angle in degrees that a pulse will be emitted")] [Min(0f)]
    [SerializeField] private float m_verticalAngle = 30f;
    [Tooltip("The max angle in degrees that a pulse will be emitted")] [Min(0f)]
    [SerializeField] private float m_horizontalAngle = 2f;
    [Tooltip("The range of the radar in nautical miles")]
    [SerializeField] private float m_range = 20f;
    private float m_nauticalMiles = 2000f;

    private readonly Collider[] m_detectedObjects = new Collider[20];

    private void Awake()
    {
        m_nauticalMiles = m_range * 100f;
    }

    private void Update()
    {
        CheckObjectsInRange();
    }

    private void CheckObjectsInRange()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, m_nauticalMiles, m_detectedObjects);
        if (size == 0)
            return;

        for (int i = 0; i < size; i++)
        {
            if (m_detectedObjects[i] is null)
            {
                continue;
            }

            Debug.DrawLine(Vector3.zero, transform.forward, Color.red);
            Vector3 directionToTarget = (m_detectedObjects[i].transform.position - transform.position).normalized;
            Debug.DrawLine(Vector3.zero, directionToTarget, Color.blue);

            // Get the pitch from radar forward to object
            Vector2 pitchFlatForward = new Vector2(transform.forward.y, transform.forward.z).normalized;
            Vector2 pitchFlatDirectionToTarget = new Vector2(directionToTarget.y, directionToTarget.z).normalized;
            float pitchDot = Vector3.Dot(pitchFlatForward, pitchFlatDirectionToTarget);
            float pitchToTarget = Mathf.Acos(pitchDot) * Mathf.Rad2Deg;

            // Get the yaw from radar forward to object
            Vector2 yawFlatForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
            Vector2 yawFlatDirectionToTarget = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
            float yawDot = Vector3.Dot(yawFlatForward, yawFlatDirectionToTarget);
            float yawToTarget = Mathf.Acos(yawDot) * Mathf.Rad2Deg;

            if (yawToTarget < m_horizontalAngle && pitchToTarget < m_verticalAngle)
            {
                Debug.Log("Target in sight");
            }
        }
    }

    private Vector3 GetRelativeDirectionOfAngle(float pitch, float yaw)
    {
        Vector3 direction = transform.forward;
        Quaternion verticalRotation = Quaternion.AngleAxis(pitch, -transform.right);
        direction = verticalRotation * direction;

        Quaternion horizontalRotation = Quaternion.AngleAxis(yaw, -transform.up);
        direction = horizontalRotation * direction;
        
        return direction.normalized;
    }
    
    private void OnDrawGizmos()
    {
        float nauticalRange = m_range * 100f;

        // Draw Range Sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, nauticalRange);
        
        // Draw scan area
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawLine(transform.position, GetRelativeDirectionOfAngle(m_verticalAngle, 0f) * nauticalRange);
        Gizmos.DrawLine(transform.position, GetRelativeDirectionOfAngle(-m_verticalAngle, 0f) * nauticalRange);
        Gizmos.DrawLine(transform.position, GetRelativeDirectionOfAngle(0f, m_horizontalAngle) * nauticalRange);
        Gizmos.DrawLine(transform.position, GetRelativeDirectionOfAngle(0f, -m_horizontalAngle) * nauticalRange);
    }
}
