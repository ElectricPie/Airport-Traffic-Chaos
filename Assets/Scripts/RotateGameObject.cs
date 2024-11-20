using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
   [Tooltip("The Rotations per minute (RPM) of the object")] [Min(0f)]
   [SerializeField] private float m_rpm = 5f;

   private void Update()
   {
      float degreesPerSecond = m_rpm * 360f / 60f;
      transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0.0f);
   }
}
