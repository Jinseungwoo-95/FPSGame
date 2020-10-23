using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 참고용 (3인칭일때 써야할 듯?)
public class Shake : MonoBehaviour
{
    [SerializeField] bool shakeRotate = false;

    [SerializeField] Vector3 originPos;
    [SerializeField] Quaternion originRot;

    void Awake()
    {
        originPos = transform.localPosition;
        originRot = transform.localRotation;
    }

    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.1f)
    {
        float passTime = 0f;
        while (passTime < duration)
        {
            Vector3 shakePos = Random.insideUnitSphere;
            transform.localPosition = shakePos * magnitudePos;

            if(shakeRotate)
            {
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0f));
                transform.localRotation = Quaternion.Euler(shakeRot);
            }

            passTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originPos;
        transform.localRotation = originRot;
    }
}
