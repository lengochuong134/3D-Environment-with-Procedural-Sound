using UnityEngine;

public class Bell : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
   void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked on: " + hit.collider.tag);
                GenerateSound(hit.collider);

            }
        }
    }


   public void GenerateSound(Collider hit)
{
    float freq = 0;

    switch (hit.tag)
    {
        case "bell": freq = 220.0f; break; // ấm và trầm như chuông chùa
    }

    if (freq > 0)
    {
        GetComponent<ChuckSubInstance>().RunCode($@"
            // Các partials mô phỏng tiếng chuông
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

            // Exponential decay (âm nhỏ dần tự nhiên)
            0.8 => float startGain;
            7::second => dur totalTime;
            now => time start;

            while (now - start < totalTime)
            {{
                Math.exp(-2.5 * (now - start) / totalTime) * startGain => mix.gain;
                10::ms => now;
            }}

            // Fade-out nhẹ để tránh click
            0.2::second => dur fade;
            mix.gain() => float g;
            now => time fadeStart;

            while (now - fadeStart < fade)
            {{
                g * (1.0 - ((now - fadeStart) / fade)) => mix.gain;
                10::ms => now;
            }}

            0 => mix.gain;
            0.1::second => now; // delay nhỏ để DAC tắt êm
        ");
    }
}


}
