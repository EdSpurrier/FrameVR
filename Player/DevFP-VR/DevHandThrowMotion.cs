using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.DevFP_VR
{
    public class DevHandThrowMotion : MonoBehaviour
    {
        [Title("Input")]
        [SerializeField] private DevFirstPersonInput input;

        [Title("Motion")]
        [SerializeField] private Transform handTransform;
        [SerializeField] private Vector3 localWindupOffset = new Vector3(0f, -0.05f, -0.2f);
        [SerializeField] private Vector3 localThrowOffset = new Vector3(0f, 0.05f, 0.35f);
        [SerializeField] private float windupTime = 0.12f;
        [SerializeField] private float throwTime = 0.08f;
        [SerializeField] private float returnTime = 0.12f;

        private Vector3 startLocalPosition;
        private Coroutine routine;

        private void Awake()
        {
            if (input == null)
                input = GetComponentInParent<DevFirstPersonInput>();

            if (handTransform == null)
                handTransform = transform;

            startLocalPosition = handTransform.localPosition;
        }

        private void Update()
        {
            if (input == null)
                return;

            if (routine == null && input.ThrowHandPressed)
                SimulateThrow();
        }

        public void SimulateThrow()
        {
            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(ThrowRoutine());
        }

        private IEnumerator ThrowRoutine()
        {
            yield return MoveTo(startLocalPosition + localWindupOffset, windupTime);
            yield return MoveTo(startLocalPosition + localThrowOffset, throwTime);
            yield return MoveTo(startLocalPosition, returnTime);

            routine = null;
        }

        private IEnumerator MoveTo(Vector3 target, float duration)
        {
            Vector3 from = handTransform.localPosition;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;

                handTransform.localPosition = Vector3.Lerp(from, target, t);
                yield return null;
            }

            handTransform.localPosition = target;
        }
    }
}