using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] private string Name = "Base";
    [SerializeField] private char DistantName = 'B';
    [SerializeField] private float Health = 100;
    private float curHealth;
    private bool alerted;

    void Start() {}

    public float dmg(float amt) {
        curHealth -= amt;
        return curHealth;
    }
}
