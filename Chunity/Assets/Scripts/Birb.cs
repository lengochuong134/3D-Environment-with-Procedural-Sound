using UnityEngine;

public class Birb : MonoBehaviour
{
    void Start()
    {
        PlayBird();
    }

    public void PlayBird()
    {
        ChuckSubInstance chuck = GetComponent<ChuckSubInstance>();
        if (chuck == null)
        {
            Debug.LogWarning("ChuckSubInstance missing! Add it to the same object.");
            return;
        }

chuck.RunCode(@"
Gain master => JCRev reverb => dac;
0.7 => reverb.mix;
0.55 => master.gain;

// Osc + filter tạo formant chim
TriOsc bird => BPF bp => ADSR env => master;
0 => bird.gain;

2000 => bp.freq;
4 => bp.Q;

// Noise rất nhẹ cho texture
Noise n => HPF h => master;
3000 => h.freq;
0 => n.gain;

// Envelope rất ngắn (chirp)
env.set(3::ms, 18::ms, 0.2, 25::ms);

while (true)
{
    // Khoảng nghỉ không đều
    Math.random2f(0.6, 2.0)::second => now;

    // Base pitch
    Math.random2(1800, 2600) => float base;

    // 50% chance double chirp
    Math.random2(0, 1) => int chirps;
    chirps + 1 => chirps;

    for (0 => int c; c < chirps; c++)
    {
        1 => env.keyOn;

        // Sweep lên nhanh
        for (0 => int i; i < 6; i++)
        {
            base + i * 120 => bird.freq;
            3::ms => now;
        }

        // Rớt nhẹ
        for (0 => int i; i < 4; i++)
        {
            base + 700 - i * 150 => bird.freq;
            4::ms => now;
        }

        // Texture noise rất nhẹ
        0.02 => n.gain;
        15::ms => now;
        0 => n.gain;

        1 => env.keyOff;
        40::ms => now;
    }
}
");


    }
}
