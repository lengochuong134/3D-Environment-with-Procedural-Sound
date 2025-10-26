using UnityEngine;

public class PickupAudio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<ChuckSubInstance>().RunCode(@"
            TriOsc spatialOsc => dac;
            while(true)
            {
                Math.random2f(300,1000) => spatialOsc.freq;
                50::ms => now;
                }
                ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
