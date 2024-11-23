using System.Collections.Generic;
using UnityEngine;

public class PrimaryRadarDisplay : MonoBehaviour
{
    [SerializeField] private PrimaryRadar m_radar;
    [SerializeField] private RectTransform m_radarWidgetTransform = null;
    
    [SerializeField] private GameObject m_radarContactPrefab;

    private Dictionary<GameObject, RectTransform> m_radarContacts = new Dictionary<GameObject, RectTransform>();
    
    private void Start()
    {
        if (m_radar is not null)
        {
            m_radar.OnTargetDetectedEvent.AddListener(OnTargetDetected);
        }
    }

    private void OnTargetDetected(GameObject target, Vector2 directionToTarget, float distanceToTarget)
    {
        Debug.Log($"New Contact @ {directionToTarget.ToString()} @ {distanceToTarget}");

        if (m_radarWidgetTransform is null || m_radar is null)
            return;

        if (m_radarContactPrefab is null)
        {
            Debug.LogError($"{name} is missing radar contact prefab");
            return;
        }
        
        float halfWidth = m_radarWidgetTransform.rect.size.x / 2f;
        float widgetWidthDistance = halfWidth / m_radar.Range;
        
        float halfHeight = m_radarWidgetTransform.rect.size.y / 2f;
        float widgetHeightDistance = halfHeight / m_radar.Range;

        if (m_radarContacts.ContainsKey(target))
        {
            // TODO: Update existing entry's position   
        }
        else
        {
            GameObject newRadarContact = Instantiate(m_radarContactPrefab, m_radarWidgetTransform);
        
            RectTransform newContactTransform = newRadarContact.transform as RectTransform;
            if (newContactTransform)
            {
                Vector2 localPosition = new Vector2(directionToTarget.x * widgetWidthDistance * distanceToTarget,
                    directionToTarget.y * widgetHeightDistance * distanceToTarget);
                Debug.Log($"Local Pos: {localPosition.ToString()}");
                newContactTransform.anchoredPosition = localPosition;
                m_radarContacts.Add(target, newContactTransform);
            }
            else
            {
                Debug.LogError("Failed to get rect transform for new contact");
                Destroy(newRadarContact);
            }
        }
    }
}
