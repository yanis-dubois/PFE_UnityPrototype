using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using Dummiesman;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class OBJImportDialog : MonoBehaviour {
    public GameObject objectSpawner; 
    GameObject loadedObject;

    private void Start() {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".obj"));
        bool isDialogShown = FileBrowser.ShowLoadDialog(OpenFileCallback, null, FileBrowser.PickMode.Files, false, null, "Select a file", "Select");
        Debug.Log("isDialogShown = " + isDialogShown);
    }

    private void OpenFileCallback(string[] paths) {
        string log = string.Empty;

        // at least one file
        if (paths.Length > 0) {
            foreach (string filePath in paths) {
                if (!File.Exists(filePath)) {
                    log = "File doesn't exist.";
                } else {
                    if (loadedObject != null)            
                        Destroy(loadedObject);
                    OpenObjFromPath(filePath);
                    log = "File loaded";
                }
            }
        }
        // no file
        else {
            log = "No file selected.";
        }

        Debug.Log(log);
    }

    private void OpenObjFromPath(string filePath) {
        var loadedObj = new OBJLoader().Load(filePath);

        // move object in the scene tree
        loadedObj.transform.parent = objectSpawner.transform;
        
        foreach (Transform child in loadedObj.transform) {
            // rescale and move object
            Vector3 size = child.GetComponent<Renderer>().bounds.size;
            float maxDim = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
            child.transform.localScale = new Vector3(1.0f/maxDim, 1.0f/maxDim, 1.0f/maxDim);
            Vector3 position = child.GetComponent<Renderer>().bounds.center;
            child.transform.position += Vector3.one - position;

            // add physics
            BoxCollider boxCollider = child.gameObject.AddComponent<BoxCollider>();
            Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            // make it interactable
            XRGrabInteractable grabInteractable = child.gameObject.AddComponent<XRGrabInteractable>();
            XRGeneralGrabTransformer grabTransformer = child.gameObject.AddComponent<XRGeneralGrabTransformer>();
            // ...

            // add material
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            child.GetComponent<Renderer>().material = mat; 
            // ...
        }
    }
}
