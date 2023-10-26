using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    private bool arrived;
    public Vector3 Orientation => transform.forward;
    private Collider fullEndCollider;

    private void Awake() { fullEndCollider = GetComponent<Collider>(); }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered");
        arrived = true;
    }
    private void OnCollisionEnter(Collision other) {
        Debug.Log("Collided");
        arrived = true;
    }
    public bool Touched() {
        return arrived;
    }
    public void ResetTouched() {
        this.arrived = false;
    }
}
