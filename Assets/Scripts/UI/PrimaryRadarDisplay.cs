using System.Collections.Generic;
using UnityEngine;

// Setup for later data
public class PrimaryRadarContact
{
    public RectTransform WidgetRectTransform { get; }
    public float LastUpdateTime;

    public PrimaryRadarContact(RectTransform widgetRectTransform)
    {
        WidgetRectTransform = widgetRectTransform;
        LastUpdateTime = Time.time;
    }
}

public class PrimaryRadarDisplay : MonoBehaviour
{
    [SerializeField] private PrimaryRadar m_radar;
    [SerializeField] private RectTransform m_radarWidgetTransform = null;

    [SerializeField] private GameObject m_radarContactPrefab;
    
    [Tooltip("The minimum amount of time between a contact updating its position")]
    [SerializeField] private float m_minimumUpdateTime = 0.5f;

    private readonly Dictionary<GameObject, PrimaryRadarContact> m_radarContacts = new Dictionary<GameObject, PrimaryRadarContact>();

    private void Start()
    {
        if (m_radar is not null)
        {
            m_radar.OnTargetDetectedEvent.AddListener(OnTargetDetected);
        }
    }

    private void OnTargetDetected(GameObject target, Vector2 directionToTarget, float distanceToTarget)
    {
        if (m_radarWidgetTransform is null || m_radar is null)
            return;

        if (m_radarContactPrefab is null)
        {
            Debug.LogError($"{name} is missing radar contact prefab", this);
            return;
        }

        // Calculate the distance of contacts on the widget vs radar range
        float halfWidth = m_radarWidgetTransform.rect.size.x / 2f;
        float widgetWidthDistance = halfWidth / m_radar.Range;
        float halfHeight = m_radarWidgetTransform.rect.size.y / 2f;
        float widgetHeightDistance = halfHeight / m_radar.Range;
        
        Vector2 widgetPosition = new Vector2(directionToTarget.x * widgetWidthDistance * distanceToTarget,
            directionToTarget.y * widgetHeightDistance * distanceToTarget);

        // Update/Create contacts
        if (m_radarContacts.TryGetValue(target, out PrimaryRadarContact contact))
        {
            // Prevent the same contact being updated too much
            if (contact.LastUpdateTime + m_minimumUpdateTime < Time.time)
            {
                contact.WidgetRectTransform.anchoredPosition = widgetPosition;
                contact.LastUpdateTime = Time.time;
            }
        }
        else
        {
            GameObject newRadarContact = Instantiate(m_radarContactPrefab, m_radarWidgetTransform);

            RectTransform newContactTransform = newRadarContact.transform as RectTransform;
            if (newContactTransform)
            {
                PrimaryRadarContact newContact = new PrimaryRadarContact(newContactTransform);
                newContactTransform.anchoredPosition = widgetPosition;
                m_radarContacts.Add(target, newContact);
            }
            else
            {
                Debug.LogError("Failed to get rect transform for new contact");
                Destroy(newRadarContact);
            }
        }
    }
    
    // TODO: Handle contact leaving radar range
}