using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GetReady : MonoBehaviour
{
    float lifetime = 8.65f;
    bool shown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shown)
        {
            lifetime -= Time.deltaTime;
            if(lifetime < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }


    public void BringToView()
    {
        shown = true;
        // Make it go large and then smaller in size
        Sequence seq = DOTween.Sequence();
        seq.Append(this.GetComponent<RectTransform>().DOScale(5f, .4f));
        seq.Append(this.GetComponent<RectTransform>().DOScale(3f, .25f));
        seq.Append(this.GetComponent<RectTransform>().DOScale(.2f, 8f));


    }

}
