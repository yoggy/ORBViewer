using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowUI : MonoBehaviour {

    public Button button_mode_change;

    float st;
    float show_duration = 5.0f;

    private void Start()
    {
        st = Time.time;
    }

    void Update () {

        if (Input.GetMouseButtonDown(0)) {
            st = Time.time;
        }

        float diff = Time.time - st;
        bool flag = false;
        if (diff < show_duration) {
            flag = true;
        }

        button_mode_change.gameObject.SetActive(flag);
	}
}
