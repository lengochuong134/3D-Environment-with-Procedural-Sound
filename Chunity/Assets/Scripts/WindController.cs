using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ChuckSubInstance))]
public class WindController : MonoBehaviour
{
    public Transform player;
    public float occlusionDistance = 0.5f; // raycast margin
    private AudioSource audioSource;
    private ChuckSubInstance chuck;
    private AudioLowPassFilter lpf;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        chuck = GetComponent<ChuckSubInstance>();
        lpf = GetComponent<AudioLowPassFilter>();

        // safe ChucK wind code (looping)
        string code = @"
            // Soft temple wind
            // brown noise is smoother and less harsh than white noise
            Noise n => OnePole smooth => LPF lp => Gain g => dac;

            // smoothing filter to make noise more 'brown'
            0.98 => smooth.pole;

            0.25 => g.gain;
            500 => lp.freq;

            // slow sine modulation for natural breathing
            SinOsc mod => blackhole;
            0.07 => mod.freq;  // slow mod
            300 => float baseFreq;
            600 => float range;

            while(true)
            {
                baseFreq + range * (0.5 + 0.5 * mod.last()) => lp.freq;
                (0.18 + 0.07 * mod.last()) => g.gain;
                100::ms => now;
            }
        ";
        chuck.RunCode(code);
    }

    void Update()
    {
        // occlusion: raycast from player to this source
        if (player == null) return;

        Vector3 dir = transform.position - player.position;
        float dist = dir.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(player.position, dir.normalized, out hit, dist))
        {
            // hit something between player and source => occluded
            audioSource.volume = 0.25f;
            lpf.cutoffFrequency = 1200f;
        }
        else
        {
            audioSource.volume = 0.45f;
            lpf.cutoffFrequency = 22000f;
        }

        // optional: set stereo spread by angle (not necessary if spatial blend=1)
    }
}
