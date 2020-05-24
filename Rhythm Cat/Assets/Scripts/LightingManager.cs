using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightingManager : MonoBehaviour
{
    public SpriteRenderer stageLights;
    public SpriteRenderer[] spotLights;
    public SpriteRenderer[] GreyCatSprites;
    public SpriteRenderer[] BlueCatSprites;
    public SpriteRenderer[] YellowCatSprites;
    public SpriteRenderer[] WhiteCatSprites;

    float duration = 6f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LightsOn()
    {
        Sequence stageseq = DOTween.Sequence();
        stageseq.Append(stageLights.DOColor(new Color(1f, 1f, 1f, 1f), 2f));
        

        foreach (SpriteRenderer s in spotLights)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(s.DOColor(new Color(1f, 1f, 1f, 1f), duration));
        }

        foreach (SpriteRenderer s in GreyCatSprites)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(s.DOColor(new Color(1f, 1f, 1f, 1f), duration));
        }
        foreach (SpriteRenderer s in BlueCatSprites)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(s.DOColor(new Color(1f, 1f, 1f, 1f), duration));
        }
        foreach (SpriteRenderer s in YellowCatSprites)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(s.DOColor(new Color(1f, 1f, 1f, 1f), duration));
        }
        foreach (SpriteRenderer s in WhiteCatSprites)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(s.DOColor(new Color(1f, 1f, 1f, 1f), duration));
        }
    }

    public void LightsOff()
    {

    }
}
