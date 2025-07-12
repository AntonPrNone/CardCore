using UnityEngine;

public class BilboardUI : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                 mainCam.transform.rotation * Vector3.up);

    }
}
