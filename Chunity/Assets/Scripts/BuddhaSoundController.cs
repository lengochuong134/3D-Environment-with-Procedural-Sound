using UnityEngine;

public class BuddhaSoundController : MonoBehaviour
{
    public Transform player;
    private ChuckSubInstance chuck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Tạo 1 instance ChucK riêng cho tượng Phật
        chuck = GetComponent<ChuckSubInstance>();

        // Gửi code ChucK sinh âm thanh liên tục
        chuck.RunCode(@"
            // === Buddha Aura Sound ===
            SinOsc s => LPF f => dac;
            200 => s.freq;      // base frequency
            1000 => f.freq;     // default filter cutoff
            0.3 => s.gain;

            // Biến điều khiển từ Unity
            global float distance;
            while(true) {
                // Thay đổi âm lượng và filter theo khoảng cách
                f.freq => float baseFreq;
                Math.exp(-distance / 5.0) => s.gain;        // xa hơn -> nhỏ hơn
                (200 + (1 - Math.exp(-distance / 8.0)) * 100) => s.freq;
                (800 + distance * 50) => f.freq;
                0.05::second => now;
            }
        ");
    }

    void Update()
    {
        if (player == null) return;

        float d = Vector3.Distance(player.position, transform.position);
        chuck.SetFloat("distance", d);
    }
}
