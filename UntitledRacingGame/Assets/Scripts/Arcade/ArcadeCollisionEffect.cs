using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeCollisionEffect : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public ArcadeCarController controller;
    public AudioSource heavyCollisionSource;
    public AudioSource lightCollisionSource;
    public AudioClip[] lightCols;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 10) // static props
        {
            if (controller.IsFast())
            {
                heavyCollisionSource.Play();
                GameObject effect = Instantiate(hitEffectPrefab, other.GetContact(0).point, Quaternion.identity);
                Destroy(effect, 5f);
            }

            controller.SetSpeed(0);
        }
        
        if (other.gameObject.layer == 9) // dynamic props
        {
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;

            if (controller.IsSomewhatFast())
            {
                lightCollisionSource.clip = lightCols[Random.Range(0, lightCols.Length)];
                lightCollisionSource.Play();
                other.gameObject.layer = 18;
                Destroy(other.gameObject, 6f);
            }
        }
    }
}
