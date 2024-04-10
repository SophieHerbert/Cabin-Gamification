using UnityEngine;

// This class is used to story and retrieve image size information  
public class ImageFix : MonoBehaviour
{
    // Stores a Vector2 to a private variable as imageSizeOnChange and sets it to the value of x(2) and y(1)    
    [SerializeField] private Vector2 imageSizeOnChange = new Vector2(2, 1);
    // This stores a Vector2 to a private variable as originalSize which stores the original size of the image
    private Vector2 originalSize;

    private void Start()
    {
        // Gets the RectTransform component's width of the image the script is on and sets it to the originalSize.x 
        originalSize.x = GetComponent<RectTransform>().rect.width;
        // Gets the RectTransform component's height of the image the script is on and sets it to the originalSize.y
        originalSize.y = GetComponent<RectTransform>().rect.height;
    }

    // I have made the Vector2 public so that it is accessible in other scripts 
    public Vector2 ImageSizeOnChange
    {
        // Gets the imageSizeOnChange value and returns it to the location ImageSizeOnChange was called from 
        get
        {
            return imageSizeOnChange;
        }
    }
    public Vector2 OriginalSize
    {
        // Gets the originalSize value and returns it to the location OriginalSize was called from
        get
        {
            return originalSize;
        }
    }

}
