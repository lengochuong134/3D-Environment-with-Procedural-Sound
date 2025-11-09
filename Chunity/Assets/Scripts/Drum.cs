using UnityEngine;

public class Drum : MonoBehaviour
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
        case "woodenfish": freq = 500.0f; break; // mõ thường tầm này
    }

    if (freq > 0)
    {
        GetComponent<ChuckSubInstance>().RunCode($@"
            // --- Wooden Fish Drum synthesis ---
            // Mô phỏng âm mõ: ngắn, khô, hơi cộng hưởng gỗ

            [1.0, 1.3, 2.1] @=> float partials[];   // cộng hưởng lệch nhẹ
            [1.0, 0.4, 0.2] @=> float gains[];      // partial cao yếu hơn

            Gain mix => JCRev reverb => dac;
            0.15 => reverb.mix; // reverb nhẹ thôi, vì mõ không vang lâu

            {freq} => float baseFreq;

            for (0 => int i; i < partials.size(); i++)
            {{
                SinOsc s => mix;
                baseFreq * partials[i] => s.freq;
                gains[i] * 0.8 => s.gain;
            }}

            // Thêm một noise nhẹ để mô phỏng va chạm gỗ
            Noise n => OnePole p => mix;
            0.02 => n.gain;
            0.9 => p.pole;

            // Envelope nhanh kiểu “cốc”
            10::ms => dur attack;
            300::ms => dur decay;
            now => time start;
            while (now - start < decay)
            {{
                // exponential decay nhanh
                Math.exp(-8.0 * (now - start) / decay) => mix.gain;
                5::ms => now;
            }}

            // cleanup
            0 => mix.gain;
            0.1::second => now;
        ");
    }
}

}
