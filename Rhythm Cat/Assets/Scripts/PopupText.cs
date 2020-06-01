using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PopupText : MonoBehaviour
{
    // How a spawned text for Good, Perfect, Missed hits etc behaves

    public float speed = 1f;
    float lifeTimer = 1f;

    public int dir = 1;
    // Start is called before the first frame update
    void Start()
    {
        // Make it grow and shrink using Tweening
        Sequence seq = DOTween.Sequence();
        seq.Append(this.GetComponent<RectTransform>().DOScale(1.7f, .4f));
        seq.Append(this.GetComponent<RectTransform>().DOScale(.1f, .9f));


    }

    // Update is called once per frame
    void Update()
    {
        // Set it to self-destruct
        lifeTimer -= Time.deltaTime;

        if (lifeTimer < 0 )
        {
            Destroy(this.gameObject);
        }

        // Move it up/down during its lifetime
        transform.Translate(Vector3.up * Time.deltaTime * speed * dir);
    }
}
