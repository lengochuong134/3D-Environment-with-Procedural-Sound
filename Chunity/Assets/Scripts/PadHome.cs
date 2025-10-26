using UnityEngine;

public class PadHome : MonoBehaviour
{
    [HideInInspector]
    public Vector3 homePos;

    void Awake()
    {
        homePos = transform.localPosition;
    }
}
