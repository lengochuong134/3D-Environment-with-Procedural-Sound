using UnityEngine;

public class BuddhaSoundController : MonoBehaviour
{
    public Transform player;
    private ChuckSubInstance chuck;

    void Start()
    {
        chuck = GetComponent<ChuckSubInstance>();

        chuck.RunCode(@"
            SinOsc s;
            LPF f;

            // Unity-ChucK ONLY accepts chain like this:
            s => f => dac;

            200 => s.freq;
            0.2 => s.gain;
            800 => f.freq;

            global float distance;

            while(true)
            {
                // Safe: Unity-ChucK supports left-arrow assignment INTO gain/freq
                0.5 * Math.exp(-distance / 5.0) => s.gain;

                500 + (1 - Math.exp(-distance / 5.0)) * 1500 => f.freq;

                20::ms => now;
            }
        ");
    }

    void Update()
    {
        float d = Vector3.Distance(player.position, transform.position);
        chuck.SetFloat("distance", d);
    }
}
