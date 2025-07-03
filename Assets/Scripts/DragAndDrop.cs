using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    Vector3 mousePosition;
    public bool held = false;
    public List<DropZone> potentialDropZones = new List<DropZone>();
    public DropZone previousDropZone;
    public MotionHandle currentMotion;
    public Camera mainCamera;
    private bool isDragEnabled = true;

    private void Start()
    {
        mainCamera = Camera.main;
        //Set previous dropzone to parent
        if (transform.parent != null && transform.parent.GetComponent<DropZone>() != null)
        {
            previousDropZone = transform.parent.GetComponent<DropZone>();
        }
    }
    /*private void Update()
    {
        Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3( Input.mousePosition.x,Input.mousePosition.y,10.0f)));
    }*/

    /*private Vector3 GetMousePos()
    {
        Vector3 pos = transform.position;
        //Vector3 pos = Input.mousePosition;
        return Camera.main.WorldToScreenPoint(new Vector3(pos.x, pos.y, 0.0f));
    }*/

    private void OnMouseDown()
    {
        if (isDragEnabled)
        {
            Debug.Log("MouseDown");
            //Cancel LMotion to Prevent Jitter
            if (currentMotion.IsActive())
            {
                currentMotion.Cancel();
            }
            held = true;
            transform.SetParent(null);
            //Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z + Input.mousePosition.z));
            //transform.position = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

            float planeY = 0;
            Transform draggingObject = transform;
            Plane plane = new Plane(Vector3.up, Vector3.up * planeY);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                draggingObject.position = ray.GetPoint(distance);
                Debug.DrawRay(ray.origin, ray.direction);
            }
        }
    }

    private void OnMouseUp()
    {
        if (held)
        {
            held = false;
            DetermineDropZone();
        }
    }

    private void OnMouseDrag()
    {
        if (isDragEnabled)
        {
            //Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z + Input.mousePosition.z));
            //transform.position = new Vector3(worldPoint.x,worldPoint.y,0.0f);

            float planeY = 0;
            Transform draggingObject = transform;
            Plane plane = new Plane(Vector3.forward, Vector3.up * planeY);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                draggingObject.position = ray.GetPoint(distance);
                Debug.DrawLine(ray.origin, ray.GetPoint(distance), Color.green);
                Debug.Log(distance);
            }
        }
    }

    private void DetermineDropZone()
    {
        DropZone closestDropZone = null;



        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Dropzone");
        Debug.Log(layerMask);
        if (Physics.Raycast(ray,out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Hit " + hit.collider.name);
            closestDropZone = hit.collider.GetComponent<DropZone>();
        }
        else
        {
            Debug.Log("No Hit");
            closestDropZone = previousDropZone;
            Debug.Log(closestDropZone.name);
        }
        Debug.DrawLine(ray.origin, ray.GetPoint(100), Color.red, 10f);
        Debug.DrawRay(ray.origin, ray.direction, Color.blue, 10f);

        SetDropZone(closestDropZone,false);
        
    }

    public void SetDropZone(DropZone targetDropZone, bool forceWhenDisabled, float duration = 0.6f)
    {
        //Go to previous zone if disabled and not forced
        if (!forceWhenDisabled && !targetDropZone.dropZoneActive)
        {
            targetDropZone = previousDropZone;
        }

        //Move to transform
        if (targetDropZone.isControlTransform)
        {
            this.transform.parent = targetDropZone.transform;
            if (currentMotion != null && currentMotion.IsPlaying())
            {
                currentMotion.Cancel();
            }
            currentMotion = LMotion.Create(transform.localPosition, Vector3.zero, duration).WithEase(Ease.OutExpo).BindToLocalPosition(transform);

            //Only Current Dropzone in Potential Drop Zones
            potentialDropZones.Clear();
            potentialDropZones.Add(targetDropZone);
        }
        else
        {
            this.transform.parent = targetDropZone.transform;
        }
        previousDropZone = targetDropZone;
    }

    public void SetDragEnabled(bool enabled)
    {
        isDragEnabled = enabled;
    }
}
