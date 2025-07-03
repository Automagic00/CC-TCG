using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class DropZone : MonoBehaviour
{
    public bool isControlTransform = false;
    private Camera _mainCam;

    [SerializeField]
    private GameObject col;

    private void Awake()
    {

        _mainCam = Camera.main;
    }

    private void Start()
    {
        //col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, 0.0f);
    }
    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with " + other.name);
        if(other.GetComponent<DragAndDrop>() != null && other.GetComponent<DragAndDrop>().held == true)
        {
            DragAndDrop dragged = other.GetComponent<DragAndDrop>();

            dragged.potentialDropZones.Add(this);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DragAndDrop>() != null && other.GetComponent<DragAndDrop>().held == true)
        {
            DragAndDrop dragged = other.GetComponent<DragAndDrop>();

            dragged.potentialDropZones.Remove(this);
        }
    }
}
