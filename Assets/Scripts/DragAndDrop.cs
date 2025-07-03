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
        if (plane.Raycast(ray,out distance))
        {
            draggingObject.position = ray.GetPoint(distance);
            Debug.DrawRay(ray.origin, ray.direction);
        }

    }

    private void OnMouseUp()
    {
        held = false;
        DetermineDropZone();
    }

    private void OnMouseDrag()
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

    private void DetermineDropZone()
    {
        DropZone closestDropZone = null;
        float closestDistance = -1;


        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float distance;
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
        //Check if there are valid dropzones in list, if not return to previous dropzone
        /*if (!potentialDropZones.Any())
        {
            closestDropZone = previousDropZone;
        }
        else
        {
            //Check Which Dropzone is Closest
            foreach (var zone in potentialDropZones)
            {
                float distToTest = Vector3.Distance(zone.transform.position, this.transform.position);
                if (closestDropZone == null || distToTest < closestDistance)
                {
                    closestDropZone = zone;
                    closestDistance = distToTest;
                }
            }
        }*/


        //Move to transform
        if (closestDropZone.isControlTransform)
        {
            this.transform.parent = closestDropZone.transform;
            currentMotion = LMotion.Create(transform.localPosition, Vector3.zero, 0.6f).WithEase(Ease.OutExpo).BindToLocalPosition(transform);
            
            //Only Current Dropzone in Potential Drop Zones
            potentialDropZones.Clear();
            potentialDropZones.Add(closestDropZone);
        }
        else
        {
            this.transform.parent = closestDropZone.transform;
        }
        previousDropZone = closestDropZone;
        
    }
}
