using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class gameObjectChangeColor : MonoBehaviour
{
    //Start is called before the first frame update
    private Renderer objectRenderer;
    private Color orginalColor;
    private Color onMouseOverColor = Color.cyan;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        orginalColor = objectRenderer.material.color; 
    }
    
    private void OnMouseEnter()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
        {
            objectRenderer.material.color = onMouseOverColor;
        }
    }

    private void OnMouseExit()
    {
        objectRenderer.material.color = orginalColor;
    }
}
