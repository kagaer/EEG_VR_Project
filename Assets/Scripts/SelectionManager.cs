using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material selectedMaterial; 
    [SerializeField] private Material defaultMaterial;

    [SerializeField] GameObject leftChair;
    [SerializeField] GameObject rightChair;

    public bool hasStarted = false;
    public bool inTrial = false;
    public bool objectSelected = false;
    public string objectHit = "";
    public float rt;
    public float startTrialTime;

    private Transform _selection;
    private Transform lastSelected; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {            
            if (inTrial)
            {
                objectHit = "";
                
                if (_selection != null)
                {
                    var selectionRenderer = _selection.GetComponent<Renderer>();
                    selectionRenderer.material = defaultMaterial;
                    _selection = null;
                }
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && !objectSelected)
                {
                    var selection = hit.transform;

                    if (selection.CompareTag(selectableTag))
                    {
                        var selectionRenderer = selection.GetComponent<Renderer>();
                        if (selectionRenderer != null)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                rt = Time.realtimeSinceStartup - startTrialTime;
                                selectionRenderer.material = selectedMaterial;
                                if (hit.transform == leftChair.transform)
                                {
                                    objectHit = "left";
                                }
                                else if (hit.transform == rightChair.transform)
                                {
                                    objectHit = "right";
                                }
                           
                                objectSelected = true;
                                inTrial = false; 
                                
                            }
                            else
                            {
                                selectionRenderer.material = highlightMaterial;
                                _selection = selection;
                            }
                            lastSelected = selection;
                        }


                    }

                }
            }
            else
            {
                //objectHit = "";
           
                    var selectionRenderer = lastSelected.GetComponent<Renderer>();
                    selectionRenderer.material = defaultMaterial;
           
            }

        }


    }
}
