using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ChuckSubInstance))]
public class WaterController : MonoBehaviour
{
    public Transform player;
    private AudioSource audioSource;
    private ChuckSubInstance chuck;
    private AudioLowPassFilter lpf;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        chuck = GetComponent<ChuckSubInstance>();
        lpf = GetComponent<AudioLowPassFilter>();

        string code = @"
            // Temple stream: gentle flowing water
            Noise n => Gain mix => dac;
            0.5 => mix.gain;

            // several moving bandpass filters to create 'bubbles'
            for (0 => int i; i < 4; i++)
            {
                BPF bp => Gain g => mix;
                n => bp;
                (800 + i*600) => bp.freq;
                2.5 => bp.Q;
                (0.4 - 0.03*i) => g.gain;

                spork ~ modulate(bp, g);
            }

            fun void modulate(BPF bp, Gain g)
            {
                while(true)
                {
                    (bp.freq() + Math.random2f(-250, 250)) => bp.freq;
                    (0.1 + Math.random2f(-0.05, 0.05)) => g.gain;
                    (80::ms + Math.random2(0,80)::ms) => now;
                }
            }
        ";
        chuck.RunCode(code);
    }

    void Update()
    {
        // Không có player thì thôi, không làm gì cả
        if (player == null) return;

        // Tính hướng từ player -> water, chỉ để xác định khoảng cách
        Vector3 dir = transform.position - player.position;
        float dist = dir.magnitude;

        // Nếu player == chính object này thì bỏ qua (tránh tự tham chiếu)
        if (player == transform || dist <= 0.01f)
            return;

        // Chỉ kiểm tra occlusion, không tác động đến transform player
        RaycastHit hit;
        bool occluded = Physics.Raycast(player.position, dir.normalized, out hit, dist);

        if (occluded)
        {
            audioSource.volume = 0.18f;
            lpf.cutoffFrequency = 1500f;
        }
        else
        {
            audioSource.volume = 0.32f;
            lpf.cutoffFrequency = 22000f;
        }
    }
}
