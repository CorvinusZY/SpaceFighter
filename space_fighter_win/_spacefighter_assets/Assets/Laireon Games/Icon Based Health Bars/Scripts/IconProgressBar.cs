using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class IconProgressBar : MonoBehaviour
{
    [SerializeField]
    CanvasScaler canvasScaler;//which canvas this is a part of, used to help scale with the screens size
    Vector2 previousWindowSize, difference = Vector2.one;
    bool wasScaling;//if the canvas was scaling with screen size

    public int MaxIcons//how many icons we display
    {
        get
        {
            return maxIcons;
        }

        set
        {
#if(UNITY_EDITOR)
            if(transform == null)
                transform = (RectTransform)base.transform;

            CheckImageContainer();//makes sure there is a mask etc for the images to be placed into
#endif

            if(mainImage != null && backingImage != null)
            {
                maxIcons = value;
                mainImageContainer.sizeDelta = new Vector2(totalWidth * MaxIcons, mainImage.rect.height);
                transform.sizeDelta = mainImageContainer.sizeDelta;

                RefreshImages();//add or remove instances of images if needed
            }
        }
    }

    [SerializeField]
    protected int maxIcons;
    public Sprite backingImage, mainImage;

    public float maxValue = 1;
    public float currentValue;

    public float Padding
    {
        get
        {
            return padding;
        }

        set
        {
#if(UNITY_EDITOR)
            if(transform == null)
                transform = (RectTransform)base.transform;
#endif

            if(mainImage != null)
            {
                padding = value;
                totalWidth = backingImage.rect.width + padding;
                totalWidth *= difference.x;//take into account the screen size if needed

                if(mainImageContainer != null)
                {
                    mainImageContainer.sizeDelta = new Vector2((totalWidth * MaxIcons) / difference.x, mainImage.rect.height);
                    transform.sizeDelta = mainImageContainer.sizeDelta;
                }

                RefreshImages();
            }
        }
    }

    [SerializeField]
    float padding;
    protected float totalWidth;

    public bool showFractions;

    public List<GameObject> backings = new List<GameObject>();
    public List<GameObject> mainImages = new List<GameObject>();

    public RectTransform mainImageContainer;//this helps with clipping;
    new RectTransform transform;

    protected void Start()
    {
        transform = (RectTransform)base.transform;

        if(mainImage != null && backingImage != null)
            Padding = Padding;//this seems redundant but its very important!! Its updates the width values so things are positioned properly on start
    }

    void Update()
    {
        #region Canvas Scaling
        if(canvasScaler == null)
            canvasScaler = GetComponentInParent<CanvasScaler>();

        if(canvasScaler != null)
            if(canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)//if this should shrink with the screen
            {
                wasScaling = true;

                difference = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);//(EditorWindow.focusedWindow.position.x, EditorWindow.focusedWindow.position.y);

                if(difference != previousWindowSize)//if the window has changed
                {
                    difference = new Vector2(difference.x / canvasScaler.referenceResolution.x, difference.y / canvasScaler.referenceResolution.y);//relative percentage to the screen size

                    previousWindowSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelWidth);//(EditorWindow.focusedWindow.position.x, EditorWindow.focusedWindow.position.y);

                    Padding = Padding;//forces the padding to update
                }
            }
            else if(wasScaling)//if the canvas mode has changed
            {
                difference = Vector2.zero;//show we now do nothing
                wasScaling = false;
            }
        #endregion

        if(mainImage != null && backingImage != null)//don't try and update when there are no images! Prevents errors
            UpdateAnimation();
    }

#if(UNITY_EDITOR)
    /// <summary>
    /// Called by the inspector to delete unused images
    /// </summary>
    public void ClearInactive()
    {
        for(int i = maxIcons; i < backings.Count; i++)//if we have removed icons
        {
            DestroyImmediate(backings[i]);//destroy the icons
            DestroyImmediate(mainImages[i]);
        }

        backings.RemoveRange(maxIcons, backings.Count - maxIcons);//clear the lists
        mainImages.RemoveRange(maxIcons, mainImages.Count - maxIcons);
    }
#endif

    /// <summary>
    /// Called when initialising or when the max icons changes
    /// </summary>
    void RefreshImages()
    {
        #region Hide Unused Images
        for(int i = maxIcons; i < backings.Count && i > -1; i++)//if we have removed icons
            if(backings[i] != null)
            {
                backings[i].SetActive(false);//then disable them
                mainImages[i].SetActive(false);
            }
        #endregion

        for(int i = 0; i < maxIcons; i++)
        {
            #region Add New Objects
            if(backings.Count <= i)//if there are not enough images in the pool
            {
                backings.Add(InstantiateBacking(i));
                mainImages.Add(InstantiateMainImage(i));
            }

            if(backings[i] == null)
                backings[i] = InstantiateBacking(i);

            if(mainImages[i] == null)
                mainImages[i] = InstantiateMainImage(i);
            #endregion

            backings[i].SetActive(true);
            mainImages[i].SetActive(true);
        }

        CheckImageContainer();

        if(mainImageContainer != transform)
            mainImageContainer.SetAsLastSibling();//render main images on top of the backings
    }

    /// <summary>
    /// Called per frame to move the mask as needed
    /// </summary>
    protected virtual void UpdateAnimation()
    {
        if(maxValue > 0)
        {
            if(showFractions)
                mainImageContainer.localPosition = new Vector3(totalWidth * (1 - (currentValue / maxValue)) * -1 * maxIcons / difference.x, 0, 0);//slide the mask. Makes for a much smoother animation
            else
                mainImageContainer.localPosition = new Vector3(totalWidth * (1 - (Mathf.Round(currentValue) / maxValue)) * -1 * maxIcons / difference.x, 0, 0);//slide the mask. Makes for a much smoother animation
        }

        PositionIcons();
    }

    protected void PositionIcons()
    {
        for(int i = 0; i < maxIcons; i++)
        {
            backings[i].transform.position = transform.position + new Vector3(totalWidth * (i + 0.5f), 0, 0);
            mainImages[i].transform.position = backings[i].transform.position;
        }
    }

    /// <summary>
    /// Creates a new instance of the backing game object
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    GameObject InstantiateBacking(int index)
    {
        GameObject backingObject = new GameObject("Backing " + index);
        backingObject.AddComponent<CanvasRenderer>();
        backingObject.AddComponent<Image>();
        backingObject.GetComponent<Image>().sprite = backingImage;
        backingObject.transform.SetParent(transform);
        backingObject.transform.localScale = Vector3.one;
        ((RectTransform)backingObject.transform).sizeDelta = new Vector2(backingImage.rect.width, backingImage.rect.height);

        return backingObject;
    }

    /// <summary>
    /// Creates a new instance of the main image game object
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected virtual GameObject InstantiateMainImage(int index)
    {
        GameObject mainImageObject = new GameObject("Icon " + index);
        mainImageObject.AddComponent<CanvasRenderer>();
        mainImageObject.AddComponent<Image>();
        mainImageObject.GetComponent<Image>().sprite = mainImage;
        mainImageObject.transform.SetParent(mainImageContainer);
        mainImageObject.transform.localScale = Vector3.one;
        ((RectTransform)mainImageObject.transform).sizeDelta = new Vector2(mainImage.rect.width, mainImage.rect.height);

        return mainImageObject;
    }

    /// <summary>
    /// Checks if the image container exists and if it doesnt then it makes one
    /// </summary>
    void CheckImageContainer()
    {
        if(mainImageContainer == null)
        {
            GameObject temp = new GameObject("Image Mask");
            temp.transform.parent = transform;
            temp.AddComponent<Image>();
            temp.AddComponent<Mask>();
            temp.GetComponent<Mask>().showMaskGraphic = false;
            mainImageContainer = temp.transform as RectTransform;
            mainImageContainer.pivot = new Vector2(0, 0.5f);
            mainImageContainer.localPosition = Vector3.zero;
        }
    }
}
