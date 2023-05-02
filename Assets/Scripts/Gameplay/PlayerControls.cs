using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private void Update()
    {
        //Used for mousing over cards in the scene. 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Do something with the hit object
            //Debug.Log("Hit object: " + hit.transform.name);
        }
    }
}
