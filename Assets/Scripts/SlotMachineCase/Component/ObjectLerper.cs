using System.Collections;
using SlotMachineCase.Manager;
using UnityEngine;

namespace SlotMachineCase.Component
{
    public class ObjectLerper : MonoBehaviour
    {
        [SerializeField] AnimationCurve linearCurve;
        [SerializeField] AnimationCurve sineCurve;
        
        public ObjectLerper(AnimationCurve linearCurve, AnimationCurve sineCurve)
        {
            this.linearCurve = linearCurve;
            this.sineCurve = sineCurve;
        }

        public void Lerp(LerpData lerpData)
        {
           StartCoroutine(LerpDataObject(lerpData));
        }
        
        private IEnumerator LerpDataObject(LerpData data)
        {
            data.obj.position = data.path[0];

            for (int i = 1; i < data.path.Length; i++)
            {
                yield return StartCoroutine(
                    MoveTowards(
                        data.obj, 
                        data.path[i], 
                        data.GetIndexDuration(i), 
                        i == data.path.Length - 1 ? Ease.Sine : Ease.Linear));

                data.OnWaypointChanged.Invoke(data.obj, i, data.path.Length);
            }

            if (data.isLast)
            {
                GameManager.Instance.CommandManager.InvokeCommand(new SlotMachineStoppedCommand());
            }
        }
    
        private IEnumerator MoveTowards(Transform obj, Vector3 targetPosition, float duration, Ease ease)
        {
            float counter = 0;

            float current = 0f, target = 1f;

            var startPosition = obj.position;
        
            while (counter < duration)
            {
                counter += Time.deltaTime;

                float t = (Mathf.Abs(current - target) / (duration - counter)) * Time.deltaTime;

                current = Mathf.MoveTowards(
                    current, 
                    target, 
                    ease == Ease.Linear ? linearCurve.Evaluate(t) : sineCurve.Evaluate(t));

                obj.position = Vector3.Lerp(startPosition, targetPosition, current);
            
                yield return null;
            }
        }

        private enum Ease
        {
            Linear,
            Sine
        }
    }
}
