using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GenericText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PulseText()
    {
        // Make it grow and shrink
        Sequence seq = DOTween.Sequence();
        seq.Append(this.GetComponent<RectTransform>().DOScale(1.3f, .2f));
        seq.Append(this.GetComponent<RectTransform>().DOScale(1f, .2f));

    }
}
