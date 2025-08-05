using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomIcon:MonoBehaviour
{
    private Image image;
    private Outline outline;

    [SerializeField] private Image portalsUp;
    [SerializeField] private Image portalsDown;
    [SerializeField] private Image portalsLeft;
    [SerializeField] private Image portalsRight;

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

        portalsUp.gameObject.SetActive(false);
        portalsDown.gameObject.SetActive(false);
        portalsLeft.gameObject.SetActive(false);
        portalsRight.gameObject.SetActive(false);
    }

    public void ChangeColor(Color color)
    {
        image.color = color;
    }

    public void SetOutLine(bool isEnable)
    {
        outline.enabled = isEnable;
    }

    public void SetPortalUI(Direction direction, bool isEnd)
    {
        Image portalImage = null;

        switch(direction)
        {
            case Direction.Up:
                portalImage = portalsUp;
                break;
            case Direction.Down:
                portalImage = portalsDown;
                break;
            case Direction.Left:
                portalImage = portalsLeft;
                break;
            case Direction.Right:
                portalImage = portalsRight;
                break;
        }

        if(portalImage != null)
        {
            portalImage.gameObject.SetActive(true);
            portalImage.color = isEnd ? Color.black : Color.white;
        }
    }
}
