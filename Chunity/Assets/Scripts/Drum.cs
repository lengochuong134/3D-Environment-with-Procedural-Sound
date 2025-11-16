using UnityEngine;

public class Drum : MonoBehaviour
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
                if (hit.collider.tag == "woodenfish")
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
        // Ví dụ phát âm thanh cho "woodenfish"
        float freq = 500f;

        if (chuck != null)
        {
            chuck.RunCode($@"
                [1.0, 1.3, 2.1] @=> float partials[];
                [1.0, 0.4, 0.2] @=> float gains[];

                Gain mix => JCRev reverb => dac;
                0.15 => reverb.mix;

                {freq} => float baseFreq;

                for (0 => int i; i < partials.size(); i++)
                {{
                    SinOsc s => mix;
                    baseFreq * partials[i] => s.freq;
                    gains[i] * 0.8 => s.gain;
                }}

                Noise n => OnePole p => mix;
                0.02 => n.gain;
                0.9 => p.pole;

                10::ms => dur attack;
                300::ms => dur decay;
                now => time start;
                while (now - start < decay)
                {{
                    Math.exp(-8.0 * (now - start) / decay) => mix.gain;
                    5::ms => now;
                }}

                0 => mix.gain;
                0.1::second => now;
            ");
        }
    }
}
