using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Setup for later data
public struct PrimaryRadarContact
{
    public GameObject WidgetGameObject { get; }
    // The game object in world the contact represents
    public GameObject ContactGameObject { get; }
    public RectTransform WidgetRectTransform { get; }
    public float LastUpdateTime;

    public PrimaryRadarContact(GameObject widgetGameObject, GameObject contactGameObject, RectTransform widgetRectTransform)
    {
        WidgetGameObject = widgetGameObject;
        ContactGameObject = contactGameObject;
        WidgetRectTransform = widgetRectTransform;
        LastUpdateTime = Time.time;
    }
}

public struct ContactPendingDestruction
{
    public PrimaryRadarContact PrimaryRadarContact { get; }
    // The Time.time the contact will be destroyed
    public float DestroyAtTime { get; }

    public ContactPendingDestruction(PrimaryRadarContact primaryRadarContact, float destroyAtTime)
    {
        PrimaryRadarContact = primaryRadarContact;
        DestroyAtTime = destroyAtTime;
    }
}

public class PrimaryRadarDisplay : MonoBehaviour
{
    [SerializeField] private PrimaryRadar m_radar;
    [SerializeField] private RectTransform m_radarWidgetTransform = null;

    [SerializeField] private GameObject m_radarContactPrefab;
    
    [Tooltip("The minimum amount of time between a contact updating its position")]
    [SerializeField] private float m_minimumUpdateTime = 0.5f;
    [Tooltip("The time a contact widget stays on screen after leaving range")]
    [SerializeField] private float m_contactDestructionTime = 5f;

    private readonly Dictionary<GameObject, PrimaryRadarContact> m_radarContacts = new Dictionary<GameObject, PrimaryRadarContact>();
    private readonly List<ContactPendingDestruction> m_contactsPendingDestruction = new List<ContactPendingDestruction>();

    private void Start()
    {
        if (m_radar is not null)
        {
            m_radar.OnObjectDetectedEvent.AddListener(OnObjectDetected);
            m_radar.OnObjectLeftRange.AddListener(OnObjectLeftRange);
        }
    }

    private void Update()
    {
        for (int i = m_contactsPendingDestruction.Count - 1; i >= 0; i--)
        {
            if (Time.time >= m_contactsPendingDestruction[i].DestroyAtTime)
            {
                Destroy(m_contactsPendingDestruction[i].PrimaryRadarContact.WidgetGameObject);
                m_radarContacts.Remove(m_contactsPendingDestruction[i].PrimaryRadarContact.ContactGameObject);
                m_contactsPendingDestruction.RemoveAt(i);
            }
        }
    }

    private void OnObjectDetected(GameObject detectedObject, Vector2 directionToTarget, float distanceToTarget)
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

        // Prevent destruction if it reenters the range 
        m_contactsPendingDestruction.RemoveAll(item => item.PrimaryRadarContact.ContactGameObject == detectedObject);

        // Update/Create contacts
        if (m_radarContacts.TryGetValue(detectedObject, out PrimaryRadarContact contact))
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
                PrimaryRadarContact newContact = new PrimaryRadarContact(newRadarContact, detectedObject, newContactTransform);
                newContactTransform.anchoredPosition = widgetPosition;
                m_radarContacts.Add(detectedObject, newContact);
            }
            else
            {
                Debug.LogError("Failed to get rect transform for new contact");
                Destroy(newRadarContact);
            }
        }
    }
    
    private void OnObjectLeftRange(GameObject objectOutOfRange)
    {
        if (m_radarContacts.TryGetValue(objectOutOfRange, out PrimaryRadarContact contact))
        {
            float destructionTime = Time.time + m_contactDestructionTime;
            ContactPendingDestruction newPendingDestruction = new ContactPendingDestruction(contact, destructionTime);
            m_contactsPendingDestruction.Add(newPendingDestruction);
            // Wait to remove it from contacts in case it renters the range
        }
    }
}