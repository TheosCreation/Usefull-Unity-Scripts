using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]

public class LinkHandlerForTMPText : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text m_Text;
    private Canvas Canvas;
    [SerializeField] private Camera cameraToCheck;

    public delegate void ClickOnLinkEvent(string keyword);
    public static event ClickOnLinkEvent OnClickedOnLinkEvent;

    private void Awake()
    {
        m_Text = GetComponent<TMP_Text>();
        Canvas = GetComponentInParent<Canvas>();

        if(Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            cameraToCheck = null;
        }
        else
        {
            cameraToCheck = Canvas.worldCamera;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 mousePos = new Vector3(eventData.position.x, eventData.position.y, 0);

        var linkTaggedText = TMP_TextUtilities.FindIntersectingLink(m_Text, mousePos, cameraToCheck);

        if (linkTaggedText == -1) return;

        TMP_LinkInfo linkInfo = m_Text.textInfo.linkInfo[linkTaggedText];

        string linkID = linkInfo.GetLinkID();

        // Check for common URL prefixes
        if (linkID.StartsWith("http://") || linkID.StartsWith("https://") || linkID.StartsWith("www."))
        {
            if (!linkID.StartsWith("http://") && !linkID.StartsWith("https://"))
            {
                linkID = "http://" + linkID; // Adding http prefix if only www is found
            }
            Application.OpenURL(linkID);
            return;
        }

        OnClickedOnLinkEvent?.Invoke(linkInfo.GetLinkText());
    }
}
