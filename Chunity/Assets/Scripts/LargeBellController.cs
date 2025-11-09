using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ChuckSubInstance))]
public class LargeBellController : MonoBehaviour
{
    public Transform player;
    public float interactRange = 3f;
    private ChuckSubInstance chuck;
    private AudioSource audioSource;
    private AudioLowPassFilter lpf;

    void Start()
    {
        chuck = GetComponent<ChuckSubInstance>();
        audioSource = GetComponent<AudioSource>();
        lpf = GetComponent<AudioLowPassFilter>();
        // no persistent code needed for bell, we run a short shred when struck
    }

    void Update()
    {
        if (player == null) return;
        // occlusion for bell
        Vector3 dir = transform.position - player.position;
        float dist = dir.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(player.position, dir.normalized, out hit, dist))
        {
            audioSource.volume = 0.6f * 0.35f; // reduced
            lpf.cutoffFrequency = 2000f;
        }
        else
        {
            audioSource.volume = 0.6f;
            lpf.cutoffFrequency = 22000f;
        }

        // interaction: press E to strike if close enough
        if (Input.GetKeyDown(KeyCode.E) && dist <= interactRange)
        {
            PlayBell(demo: false);
        }
    }

    public void PlayBell(bool demo = false)
    {
        // ChucK code that creates several partials with exponential decay envelopes
        string code = @"
            // Large bell strike: multi-partial sine + ADSR
            fun void strike()
            {
                // partial frequencies relative to a base
                [1.0, 2.7, 3.9, 5.2, 7.1] @=> float partialRatios[];
                [1.0, 0.6, 0.5, 0.35, 0.2] @=> float partialAmps[];

                // base frequency (low fundamental) - adjust for big bell
                110 => float baseFreq;

                // master gain
                Gain masterGain;
                masterGain => dac;
                0.9 => masterGain.gain; // loud

                // create UGens for each partial
                for (0 => int i; i < partialRatios.cap(); i++)
                {
                    SinOsc s => ADSR env => Gain g => masterGain;
                    (baseFreq * partialRatios[i]) => s.freq;
                    (partialAmps[i]) => g.gain;
                    // set envelope: short attack, medium decay, sustain low, long release
                    env.set(10::ms, 700::ms, 0.1, 3000::ms);
                    env.keyOn();
                    // slight detune random for realism
                    Math.random2f(-0.5, 0.5) => float det;
                    (s.freq() + det) => s.freq;
                }

                // let them ring (long decay)
                5::second => now;

                // release envelopes
                // (they will naturally die when shred exits)
            }

            // spawn a shred to play once
            strike();
        ";

        chuck.RunCode(code);
        if (demo) Debug.Log("Bell played (demo).");
    }
}
