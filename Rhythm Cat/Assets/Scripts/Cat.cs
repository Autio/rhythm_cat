using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cat : MonoBehaviour
{
    // Governs what an animated cat should do
    public GameObject mouth;
    public GameObject movingMouth;
    public Animator anim;
    public bool growing = false;

    public float duration = .3f;
    public float scaleBoost = 1.5f;

    public GameObject noteParticles;

    float growScale = 1.1f;

    public void RegularAnim()
    {

        if (transform.localScale.x == growScale)
        {
            try
            {
                anim.Play("WhiteCatSinging");
                Sequence seq = DOTween.Sequence();
                seq.Append(this.GetComponent<Transform>().DOScale(1f, 1.3f));
                growing = false;
            }
            catch
            {

            }
        }
    }
    public void LongNoteAnim()
    {
        // Should only fire once
        try
        {
            anim.Play("WhiteCatHoldNote");

            if (!growing)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(this.GetComponent<Transform>().DOScale(growScale, 1.1f));
                growing = true;
            }

        }
        catch
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x == 1f)
        {
            growing = false;
        }
        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    LongNoteAnim();
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    RegularAnim();
        //}
    }

    public void PlayNote()
    {

        // Instantiate and play new particle system
        GameObject newParticles = Instantiate(noteParticles, noteParticles.transform.position, Quaternion.identity);
        newParticles.GetComponent<ParticleSystem>().Play();
        Destroy(newParticles, 1.1f);
    }

}
