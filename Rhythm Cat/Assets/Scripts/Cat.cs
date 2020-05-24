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
        // FIXME: Hack, always sync the openable mouth and the animation one
        try
        {

           // movingMouth.transform.rotation = new Quaternion(mouth.transform.rotation.x, mouth.transform.rotation.y, mouth.transform.rotation.z - 90, mouth.transform.rotation.w);
           // movingMouth.transform.position = mouth.transform.position;
            Debug.Log("Syncing mouths");
        }
        catch
        {

        }

    }

    public void PlayNote()
    {
        // Open and close the mouth
        Sequence seq = DOTween.Sequence();

        try
        {
            seq.Append(movingMouth.GetComponent<Transform>().DOScale(scaleBoost, duration / 2));
            seq.Append(movingMouth.GetComponent<Transform>().DOScale(1f, duration / 2));
        } catch
        {

        }
        
        // Instantiate and play new particle system
        GameObject newParticles = Instantiate(noteParticles, noteParticles.transform.position, Quaternion.identity);
        newParticles.GetComponent<ParticleSystem>().Play();
        Destroy(newParticles, 1.1f);
    }

}
