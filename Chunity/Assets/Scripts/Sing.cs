using UnityEngine;
using System.Collections;


public class Sing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    
    IEnumerator PressKey(Transform key)
{
    Vector3 originalPos = key.position;
    Vector3 pressedPos = originalPos + new Vector3(0, -0.1f, 0); 


    float t = 0f;
    while (t < 1f)
    {
        t += Time.deltaTime * 10f; 
        key.position = Vector3.Lerp(originalPos, pressedPos, t);
        yield return null;
    }

    yield return new WaitForSeconds(0.1f);

    t = 0f;
    while (t < 1f)
    {
        t += Time.deltaTime * 10f;
        key.position = Vector3.Lerp(pressedPos, originalPos, t);
        yield return null;
    }

    key.position = originalPos;
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
                StartCoroutine(PressKey(hit.transform));

            }
        }
    }
}