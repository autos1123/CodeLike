using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomIcon:MonoBehaviour
{
    private Image image;
    private Outline outline;

    [SerializeField] private GameObject portalsUp;
    [SerializeField] private GameObject portalsDown;
    [SerializeField] private GameObject portalsLeft;
    [SerializeField] private GameObject portalsRight;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void ResetUI()
    {
        image.color = Color.clear;
        outline.enabled = false;

        portalsUp.SetActive(false);
        portalsDown.SetActive(false);
        portalsLeft.SetActive(false);
        portalsRight.SetActive(false);
    }

    public void ChangeColor(Color color)
    {
        image.color = color;
    }

    public void SetOutLine(bool isEnable)
    {
        outline.enabled = isEnable;
    }

    public void SetPortalUI(Direction direction)
    {
        switch(direction)
        {
            case Direction.Up:
                portalsUp.SetActive(true);
                break;
            case Direction.Down:
                portalsDown.SetActive(true);
                break;
            case Direction.Left:
                portalsLeft.SetActive(true);
                break;
            case Direction.Right:
                portalsRight.SetActive(true);
                break;
        }
    }
}
