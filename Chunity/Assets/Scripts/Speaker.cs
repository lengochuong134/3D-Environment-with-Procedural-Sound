using UnityEngine;

public class Speaker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked on: " + hit.collider.tag);
                if (hit.collider.CompareTag("speaker"))
                {
                    PlayZenMusic();
                }
                else if (hit.collider.CompareTag("birb"))
                {
                    Birb();
                }
            }
        }
    }

    public void PlayZenMusic()
    {
        ChuckSubInstance chuck = GetComponent<ChuckSubInstance>();
        if (chuck == null)
        {
            Debug.LogWarning("ChuckSubInstance missing! Add it to the same object.");
            return;
        }

        chuck.RunCode(@"
    Gain master => JCRev reverb => dac;
    0.8 => reverb.mix;
    0.5 => master.gain;

    // Melody kiểu chuông thiền
    SinOsc bell => master;

    [392, 440, 494, 523, 587, 659, 784] @=> int notes[]; // thang âm nhẹ

    time start;
    now => start;

    while (now - start < 10::second)
    {
        notes[Math.random2(0, notes.cap()-1)] => bell.freq;

        // 'ting' nhẹ có độ vang
        0.3 => bell.gain;
        0.05::second => now;
        0.15 => bell.gain;
        0.1::second => now;
        0.05 => bell.gain;
        0.3::second => now;
        0 => bell.gain;

        Math.random2f(0.5, 1.5)::second => now;
    }

    // fade out nhẹ
    for (0 => int i; i < 20; i++) {
        master.gain() * 0.9 => master.gain;
        100::ms => now;
    }
    0 => master.gain;
");



    }

    public void Birb()
    {
        ChuckSubInstance chuck = GetComponent<ChuckSubInstance>();
        if (chuck == null)
        {
            Debug.LogWarning("ChuckSubInstance missing! Add it to the same object.");
            return;
        }

        chuck.RunCode(@"
Gain master => JCRev reverb => dac;
0.6 => reverb.mix;
0.6 => master.gain;

// Chim chính
SinOsc bird => master;
0 => bird.gain;

// White noise để tạo chút rung lắc
Noise n => LPF f => master;
2000 => f.freq;
0 => n.gain;

time start;
now => start;

// Chạy 10 giây
while (now - start < 10::second)
{
    // Ngẫu nhiên khoảng cách giữa các tiếng
    Math.random2f(0.3, 1.0)::second => now;

    // Chọn tần số note ngẫu nhiên (cao, sáng)
    [1200, 1400, 1600, 1800, 2000] @=> int notes[];
    notes[Math.random2(0, notes.cap()-1)] => bird.freq;

    // Envelope chim
    0.3 => bird.gain;
    0.05::second => now;
    0.2 => bird.gain;
    0.07::second => now;
    0.0 => bird.gain;

    // Noise rung lắc
    0.05 => n.gain;
    0.07::second => now;
    0 => n.gain;
}

// fade out
for (0 => int i; i < 20; i++) {
    master.gain() * 0.9 => master.gain;
    100::ms => now;
}
0 => master.gain;
");
    }

}
