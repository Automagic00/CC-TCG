using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandLayout : MonoBehaviour
{
    private Vector3 handBounds;
    
    [SerializeField]
    private BoxCollider col;

    int childrenCount;
    private MotionHandle currentMotion;

    private void Start()
    {
        handBounds = col.bounds.size;
        UpdateHandLayout();
        childrenCount = transform.childCount;
    }

    private void Update()
    {

        if (childrenCount != transform.childCount)
        {
            //Card was added, adjust position to closest between cards
            if (childrenCount < transform.childCount)
            {
                float newCardXPos = transform.GetChild(transform.childCount - 1).transform.localPosition.x;
                for(int i = 0; i<transform.childCount - 1; i++)
                {
                    if (transform.GetChild(i).localPosition.x > newCardXPos)
                    {
                        transform.GetChild(transform.childCount - 1).transform.SetSiblingIndex(i);
                        break;
                    }
                }
            }

            UpdateHandLayout();
            childrenCount = transform.childCount;
        }
    }

    private void UpdateHandLayout()
    {
        for( int i = 0; i < transform.childCount; i++)
        {
            //Skip if not a card
            if (transform.GetChild(i).GetComponent<Card>() == null)
            {
                continue;
            }
            float offset = (((float)i + 0.5f) - (transform.childCount / 2.0f ) );
            float cardBounds = transform.GetChild(i).GetComponent<BoxCollider2D>().bounds.size.x;

            //Set front to back position
            float zPos = 0.03f * offset;

            //Check if cards would go outside of the hand bounds
            if (cardBounds * transform.childCount > handBounds.x)
            {
                float offsetMod = (cardBounds * transform.childCount) / handBounds.x;
                offset = offset / offsetMod;
            }

            float xPos = cardBounds * offset;

            DragAndDrop drag = transform.GetChild(i).GetComponent<DragAndDrop>();

            drag.currentMotion = LMotion.Create(transform.GetChild(i).localPosition, new Vector3(xPos, 0, zPos), 0.6f).WithEase(Ease.OutExpo).BindToLocalPosition(transform.GetChild(i));
            //transform.GetChild(i).localPosition = new Vector3(xPos, 0, zPos);
        }
    }
}
