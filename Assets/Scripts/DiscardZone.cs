using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class DiscardZone : MonoBehaviour
{
    int cardsInDiscard = 0;
    private void Start()
    {
        
    }

    private void Update()
    {
        var cards = GetComponentsInChildren<Card>();

        if (cards.Length > cardsInDiscard)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                DragAndDrop drag = cards[i].GetComponent<DragAndDrop>();
                drag.SetDragEnabled(false);
                //cards[i].transform.localPosition = new Vector3(0, 0, -0.1f * i);
                //cards[i].transform.SetParent(transform);

                if (drag.currentMotion != null && drag.currentMotion.IsPlaying())
                {
                    drag.currentMotion.Cancel();
                }
                drag.currentMotion = LMotion.Create(cards[i].transform.localPosition, new Vector3(0, 0, -0.1f * i), 0.6f).WithEase(Ease.OutExpo).BindToLocalPosition(cards[i].transform);
                
            }
        }
    }
}
