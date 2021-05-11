using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}
public class GestureDetection : MonoBehaviour
{
    public float threshold = 0.1f;
    public bool debugMode = true;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBone;
    private Gesture previousGesture;
    public TMPro.TextMeshProUGUI label;
    // Start is called before the first frame update
    void Start()
    {
        previousGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {
        if(debugMode && Input.GetKeyDown(KeyCode.Space)){
            fingerBone = new List<OVRBone>(skeleton.Bones);
            Save();
        }else{
            Gesture currentGesture = Recognize();
            bool hasRecognized = !currentGesture.Equals(new Gesture());

            if(hasRecognized && !currentGesture.Equals(previousGesture)){
                print("new Gesture");
                previousGesture = currentGesture;
                label.text = currentGesture.name;
                // currentGesture.onRecognized.Invoke();
            }
        }
    }
    void Save(){
        print("save");
        Gesture g = new Gesture();
        g.name = "new Gesture";
        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBone)
        {
            print(bone.Transform.position);
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerDatas = data;
        gestures.Add(g);
    }
    Gesture Recognize(){
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;
        fingerBone = new List<OVRBone>(skeleton.Bones);

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBone.Count; i++)
            {
                Vector3 currenData = skeleton.transform.InverseTransformPoint(fingerBone[i].Transform.position);
                float distance = Vector3.Distance(currenData, gesture.fingerDatas[i]);
                if(distance > threshold){
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if(!isDiscarded && sumDistance < currentMin){
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }
}
