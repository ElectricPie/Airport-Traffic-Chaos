using UnityEngine;

public class ForwardArrowGizmo : MonoBehaviour
{
    [SerializeField] private float m_arrowLength = 1f;
    [SerializeField] private Color m_arrowColor = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = m_arrowColor;
        
        Vector3 arrowEnd = transform.position + transform.forward * m_arrowLength;
        
        Gizmos.DrawLine(transform.position, arrowEnd);
        Gizmos.DrawSphere(arrowEnd, 0.05f);
    }
}
