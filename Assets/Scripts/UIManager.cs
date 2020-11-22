using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour{

    [SerializeField] private GameObject playText;

    [SerializeField] private GameObject loseText;

    public void showHidePlayText(bool state) {
        playText.SetActive(state);
    }

    public void showHideLoseScreen(bool state) {
        loseText.SetActive(state);
    }

}
