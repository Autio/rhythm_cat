using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PopupText : MonoBehaviour
{
    float speed = 1f;
    float lifeTimer = 1f;
    // Start is called before the first frame update
    void Start()
    {
        // Make it grow and shrink
        Sequence seq = DOTween.Sequence();
        seq.Append(this.GetComponent<RectTransform>().DOScale(1.7f, .4f));

        seq.Append(this.GetComponent<RectTransform>().DOScale(.1f, .9f));


    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer -= Time.deltaTime;

        if (lifeTimer < 0 )
        {
            Destroy(this.gameObject);
        }

        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }
}
