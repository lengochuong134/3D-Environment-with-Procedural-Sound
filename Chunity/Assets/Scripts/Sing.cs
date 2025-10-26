using UnityEngine;
using System.Collections;

public class Sing : MonoBehaviour
{
    void Start()
    {
        ChuckMainInstance chuck = GameObject.FindObjectOfType<ChuckMainInstance>();
        if (chuck != null)
        {
            Debug.Log("✅ Found Chuck Main Instance");
            chuck.RunCode(@"
                SinOsc s => dac;
                440 => s.freq;
                0.5 => s.gain;
                0.3::second => now;
            ");
        }
        else Debug.LogError("❌ No Chuck Main Instance found!");
    }

    IEnumerator PressKey(Transform key)
    {
        if (key == null) yield break;

        // ✅ Lấy vị trí gốc từ PadHome (ổn định khi nhấn nhiều lần)
        PadHome home = key.GetComponent<PadHome>();
        Vector3 orig = home != null ? home.homePos : key.localPosition;
        Vector3 down = orig + new Vector3(0, -0.03f, 0);

        // ✅ Dừng mọi animation PressKey khác của chính pad này
        StopCoroutine(nameof(PressKey));

        float t = 0f;
        float speedDown = 15f;
        float speedUp = 10f;

        // nhấn xuống
        while (t < 1f)
        {
            t += Time.deltaTime * speedDown;
            key.localPosition = Vector3.Lerp(orig, down, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        // trở lại
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speedUp;
            key.localPosition = Vector3.Lerp(down, orig, t);
            yield return null;
        }

        key.localPosition = orig;
    }

    void Update()
    {
        // Mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.Log("✅ Ray hit object: " + hit.collider.name + " | Tag: " + hit.collider.tag);
                GenerateSound(hit.collider);
                StartCoroutine(nameof(PressKey), hit.transform);
                Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);
            }
            else
            {
                Debug.Log("❌ Raycast did not hit anything!");
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);
            }
        }

        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Z)) PlayPad("Kick", null);
        if (Input.GetKeyDown(KeyCode.X)) PlayPad("Snare", null);
        if (Input.GetKeyDown(KeyCode.C)) PlayPad("HiHat", null);
        if (Input.GetKeyDown(KeyCode.V)) PlayPad("Tom1", null);
        if (Input.GetKeyDown(KeyCode.B)) PlayPad("Tom2", null);
        if (Input.GetKeyDown(KeyCode.N)) PlayPad("FloorTom", null);
        if (Input.GetKeyDown(KeyCode.M)) PlayPad("Crash", null);
        if (Input.GetKeyDown(KeyCode.Comma)) PlayPad("Ride", null);
    }

    public void GenerateSound(Collider hit)
    {
        if (hit == null) return;
        PlayPad(hit.tag, hit.transform);
    }

    public void PlayPad(string tagName, Transform pad)
    {
        if (pad != null)
        {
            StopCoroutine(nameof(PressKey));
            StartCoroutine(nameof(PressKey), pad);
        }

        string code = "";

        switch (tagName)
        {
            case "Kick":
                code = @"
                // Kick drum: sine + transient click
                SinOsc s => Gain g1 => ADSR env => dac;
                Noise n => BPF bp => Gain g2 => env;
                120 => s.freq;
                3000 => bp.freq;
                3 => bp.Q;
                0.9 => g1.gain;
                0.5 => g2.gain;
                env.set(5::ms, 120::ms, 0, 200::ms);
                env.keyOn();
                // pitch sweep
                120 => float start;
                50 => float end;
                for (0 => int i; i < 40; i++) {
                    (start - (start - end) * (i / 40.0)) => s.freq;
                    3::ms => now;
                }
                env.keyOff();
                200::ms => now;
            ";
                break;

            case "Snare":
                code = @"
                // Snare: noise + tone
                Noise n => HPF h => LPF l => Gain g1 => ADSR env => dac;
                SinOsc s => Gain g2 => env;
                8000 => h.freq;
                2000 => l.freq;
                180 => s.freq;
                0.5 => g1.gain;
                0.4 => g2.gain;
                env.set(3::ms, 100::ms, 0, 120::ms);
                env.keyOn();
                250::ms => now;
                env.keyOff();
            ";
                break;

            case "HiHat":
                code = @"
                // Hi-hat: bright short noise burst
                Noise n => BPF bp => ADSR env => dac;
                9000 => bp.freq;
                5 => bp.Q;
                0.4 => n.gain;
                env.set(2::ms, 30::ms, 0, 40::ms);
                env.keyOn();
                80::ms => now;
                env.keyOff();
            ";
                break;

            case "Tom1":
                code = @"
                // High tom
                SinOsc s => ADSR env => dac;
                400 => s.freq;
                0.8 => s.gain;
                env.set(3::ms, 100::ms, 0, 200::ms);
                env.keyOn();
                300::ms => now;
                env.keyOff();
            ";
                break;

            case "Tom2":
                code = @"
                // Mid tom
                SinOsc s => ADSR env => dac;
                300 => s.freq;
                0.8 => s.gain;
                env.set(3::ms, 120::ms, 0, 250::ms);
                env.keyOn();
                350::ms => now;
                env.keyOff();
            ";
                break;

            case "FloorTom":
                code = @"
                // Low tom
                SinOsc s => ADSR env => dac;
                180 => s.freq;
                0.9 => s.gain;
                env.set(5::ms, 150::ms, 0, 300::ms);
                env.keyOn();
                400::ms => now;
                env.keyOff();
            ";
                break;

            case "Crash":
                code = @"
                // Crash cymbal: long bright noise
                Noise n => BPF bp => ADSR env => dac;
                6000 => bp.freq;
                2.5 => bp.Q;
                0.9 => n.gain;
                env.set(3::ms, 200::ms, 0, 1200::ms);
                env.keyOn();
                900::ms => now;
                env.keyOff();
                400::ms => now;
            ";
                break;

            case "Ride":
                code = @"
                // Ride cymbal: subtle noise + metallic tone
                Noise n => BPF bp => ADSR env => dac;
                8500 => bp.freq;
                1.5 => bp.Q;
                0.6 => n.gain;
                env.set(5::ms, 250::ms, 0, 700::ms);
                env.keyOn();
                1::second => now;
                env.keyOff();
            ";
                break;

            default:
                Debug.Log("⚠️ Unknown drum tag: " + tagName);
                break;
        }

        if (!string.IsNullOrEmpty(code))
        {
            ChuckMainInstance chuck = GameObject.Find("TheChuck").GetComponent<ChuckMainInstance>();
            if (chuck == null)
            {
                Debug.LogError("❌ No ChuckMainInstance found in scene! Please add TheChuck prefab.");
                return;
            }
            Debug.Log("🎵 Running ChucK code for " + tagName);
            chuck.RunCode(code);
        }
    }
}
