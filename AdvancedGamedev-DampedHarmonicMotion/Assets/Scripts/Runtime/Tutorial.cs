using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{


    public GameObject equilibriumLine;
    public GameObject startLine;

    public GameObject equilibriumText;
    public GameObject startText;

    public GameObject amplitudeArrows;
    public GameObject amplitudeText;

    public GameObject smhText;
    public GameObject expText;

    private int state = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.state++;

            switch(this.state)
            {
                case 1:
                    this.equilibriumLine.SetActive(true);
                    this.equilibriumText.SetActive(true);
                    break;
                case 2:
                    this.startLine.SetActive(true);
                    this.startText.SetActive(true);
                    break;
                case 3:
                    this.amplitudeText.SetActive(true);
                    this.amplitudeArrows.SetActive(true);
                    break;
                case 4:
                    this.smhText.SetActive(true);
                    break;
                case 5:
                    this.expText.SetActive(true);
                    break;
            }
        }


    }
}
