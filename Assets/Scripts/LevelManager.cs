using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] slingBases;

    public GameObject GetNearestSlingBase(Vector3 carPosition) {
        foreach (GameObject slingBase in slingBases) 
            if (Vector3.Distance(carPosition, slingBase.transform.position) < 4.6f) 
                return slingBase;

        return null;
    }
}
