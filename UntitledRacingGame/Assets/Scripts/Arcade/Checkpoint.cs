using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private CheckpointSystem cps;
    private Animator anim;
    [SerializeField] GameObject turnIndicatorNew;
    [SerializeField] GameObject turnIndicatorOld;

    private void Awake()
    {
        cps = GetComponentInParent<CheckpointSystem>();
        anim = GetComponent<Animator>();
        HideMesh();
        if (turnIndicatorNew) turnIndicatorNew.SetActive(false);
        if (turnIndicatorOld) turnIndicatorOld.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) // car
        {
            cps.ThroughCheckpont(this);

            if (turnIndicatorNew) turnIndicatorNew.SetActive(true);
            if (turnIndicatorOld) turnIndicatorOld.SetActive(false);
        }
    }

    public void ShowMesh() { anim.SetBool("blink", true); }

    public void HideMesh() { anim.SetBool("blink", false); }
}
