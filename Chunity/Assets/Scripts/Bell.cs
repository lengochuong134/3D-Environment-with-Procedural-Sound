using UnityEngine;

public class Bell : MonoBehaviour
{
    private Animator animator;
    private ChuckSubInstance chuck;

    void Start()
    {
        animator = GetComponent<Animator>();
        chuck = GetComponent<ChuckSubInstance>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "bell")
                {
                    Debug.Log("Clicked on: " + hit.collider.tag);

                    // Trigger animation ngay khi click
                    animator.SetTrigger("PlayAnim");

                    // Phát âm thanh sau 1 giây
                    Invoke(nameof(PlaySound), 1f);
                }
            }
        }
    }

    void PlaySound()
    {
        float freq = 220f; // Chuông

        if (chuck != null)
        {
            chuck.RunCode($@"
                [1.0, 2.01, 2.74, 3.76, 4.07, 5.0] @=> float partials[];
                [1.0, 0.5, 0.3, 0.2, 0.15, 0.1] @=> float gains[];

                Gain mix => JCRev reverb => dac;
                0.35 => reverb.mix;

                {freq} => float baseFreq;

                for (0 => int i; i < partials.size(); i++)
                {{
                    SinOsc s => mix;
                    baseFreq * partials[i] => s.freq;
                    gains[i] * 0.8 => s.gain;
                }}

                0.8 => float startGain;
                7::second => dur totalTime;
                now => time start;

                while (now - start < totalTime)
                {{
                    Math.exp(-2.5 * (now - start) / totalTime) * startGain => mix.gain;
                    10::ms => now;
                }}

                0.2::second => dur fade;
                mix.gain() => float g;
                now => time fadeStart;

                while (now - fadeStart < fade)
                {{
                    g * (1.0 - ((now - fadeStart) / fade)) => mix.gain;
                    10::ms => now;
                }}

                0 => mix.gain;
                0.1::second => now;
            ");
        }
    }
}
