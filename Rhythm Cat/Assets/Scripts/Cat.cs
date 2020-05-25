using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cat : MonoBehaviour
{
    public GameObject mouth;
    public GameObject movingMouth;

    public float duration = .3f;
    public float scaleBoost = 1.5f;

    public GameObject noteParticles;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void PlayNote()
    {

        // Instantiate and play new particle system
        GameObject newParticles = Instantiate(noteParticles, noteParticles.transform.position, Quaternion.identity);
        newParticles.GetComponent<ParticleSystem>().Play();
        Destroy(newParticles, 1.1f);
    }

}
