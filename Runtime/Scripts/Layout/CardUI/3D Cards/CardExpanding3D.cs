/// <summary>
/// Credit - ryanslikesocool 
/// Sourced from - https://github.com/ryanslikesocool/Unity-Card-UI
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Extensions
{
    
[ExecuteInEditMode]
public class CardExpanding3D : MonoBehaviour
{
    [SerializeField]
    private float lerpSpeed = 12;
    [SerializeField]
    private float cornerSize = 64;

    [Header("Parts")]
    public RectTransform[] cardCorners;
    public RectTransform[] cardEdges;
    public RectTransform cardCenter;

    [Header("Card Info")]
    [Tooltip("Positions and sizes card to its current transform.")]
    public bool cardAutoSize = true;
    public Vector2 cardSize;
    public Vector2 cardPosition;
    [Range(1, 96)]
    public int cardSuperness = 4;

    [Header("Page Info")]
    [Tooltip("Positions and sizes the page to the top third of the screen.")]
    public bool pageAutoSize = true;
    public Vector2 pageSize;
    public Vector2 pagePosition;
    [Range(1, 96)]
    public int pageSuperness = 96;

    ///Just like with the 2D version of this script, I don't recommend touching this.
    private int animationActive = 0;

    private Vector2[] nextCornerPos = new Vector2[4];
    private Vector2[] nextEdgePos = new Vector2[4];
    private Vector2[] nextEdgeScale = new Vector2[4];
    private Vector2 nextCenterScale;
    private Vector2 nextPos;
    private int nextSuperness;

    private RectTransform rect;
    private Vector2 nextMin;
    private Vector2 nextMax;

    void Start()
    {
        if (cardAutoSize)
        {
            cardSize = new Vector2(cardCorners[0].localScale.x * 2 + cardEdges[0].localScale.x, cardCorners[0].localScale.y * 2 + cardEdges[0].localScale.y);
            cardPosition = cardCenter.localPosition;
        }

        if (pageAutoSize)
        {
            pageSize = new Vector2(Screen.width, Screen.height / 3);
            pagePosition = new Vector2(0, Screen.height / 2 - pageSize.y / 2);
        }

        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (animationActive == 1 || animationActive == -1)
        {
            ///Lerps the corners to new positions and supernesses.
            for (int i = 0; i < cardCorners.Length; i++)
            {
                cardCorners[i].localPosition = Vector3.Lerp(cardCorners[i].localPosition, nextCornerPos[i], Time.deltaTime * lerpSpeed);

                cardCorners[i].GetComponent<SuperellipsePoints>().superness = Mathf.Lerp(cardCorners[i].GetComponent<SuperellipsePoints>().superness, nextSuperness, Time.deltaTime * lerpSpeed);

                ///Forces everything to either the card layout or the page layout once the superness is similar enough.
                if (Mathf.Abs(cardCorners[i].GetComponent<SuperellipsePoints>().superness - nextSuperness) <= 1)
                {
                    cardCorners[i].localPosition = nextCornerPos[i];
                    cardEdges[i].localPosition = nextEdgePos[i];
                    cardEdges[i].localScale = new Vector3(nextEdgeScale[i].x, nextEdgeScale[i].y, 1);
                    transform.localPosition = nextPos;
                    cardCenter.localScale = new Vector3(nextCenterScale.x, nextCenterScale.y, 1);
                    cardCorners[i].GetComponent<SuperellipsePoints>().superness = nextSuperness;
                    rect.offsetMin = nextMin;
                    rect.offsetMax = nextMax;
                }
            }

            ///Lerps the edges to new positions and sizes.
            for (int i = 0; i < cardEdges.Length; i++)
            {
                cardEdges[i].localPosition = Vector3.Lerp(cardEdges[i].localPosition, nextEdgePos[i], Time.deltaTime * lerpSpeed);
                cardEdges[i].localScale = Vector3.Lerp(cardEdges[i].localScale, new Vector3(nextEdgeScale[i].x, nextEdgeScale[i].y, 1), Time.deltaTime * lerpSpeed);
            }

            ///Lerps the center to new position and size.
            transform.localPosition = Vector3.Lerp(transform.localPosition, nextPos, Time.deltaTime * lerpSpeed);
            cardCenter.localScale = Vector3.Lerp(cardCenter.localScale, new Vector3(nextCenterScale.x, nextCenterScale.y, 1), Time.deltaTime * lerpSpeed);

            ///Lerps the RectTransform.
            rect.offsetMin = Vector3.Lerp(rect.offsetMin, nextMin, Time.deltaTime * lerpSpeed);
            rect.offsetMax = Vector3.Lerp(rect.offsetMax, nextMax, Time.deltaTime * lerpSpeed);
        }
    }

    public void ToggleCard()
    {
        if (animationActive != 1 || animationActive == 0)
        {
            animationActive = 1;

            ///Gets new corner positions.
            for (int i = 0; i < cardCorners.Length; i++)
            {
                float posX = pageSize.x / 2 * Mathf.Sign(cardCorners[i].localScale.x) - cardCorners[i].localScale.x;
                float posY = pageSize.y / 2 * Mathf.Sign(cardCorners[i].localScale.y) - cardCorners[i].localScale.y;

                nextCornerPos[i] = new Vector2(posX, posY);
            }

            ///Same concept as the last loop.
            for (int i = 0; i < cardEdges.Length; i++)
            {
                float posX = 0;
                float posY = 0;

                float scaleX = 0;
                float scaleY = 0;

                if (cardEdges[i].localPosition.x != 0)
                {
                    posX = Mathf.Sign(cardEdges[i].localPosition.x) * ((pageSize.x / 2) - (cardEdges[i].localScale.x / 2));
                    posY = 0;

                    scaleX = cornerSize;
                    scaleY = pageSize.y - cornerSize * 2;
                }
                else if (cardEdges[i].localPosition.y != 0)
                {
                    posX = 0;
                    posY = Mathf.Sign(cardEdges[i].localPosition.y) * ((pageSize.y / 2) - (cardEdges[i].localScale.y / 2));

                    scaleX = pageSize.x - cornerSize * 2;
                    scaleY = cornerSize;
                }

                nextEdgePos[i] = new Vector2(posX, posY);
                nextEdgeScale[i] = new Vector2(scaleX, scaleY);
            }

            nextCenterScale = pageSize - new Vector2(cornerSize * 2, cornerSize * 2);
            nextPos = pagePosition;

            nextSuperness = pageSuperness;

            nextMin = new Vector2(-pageSize.x / 2, -pageSize.y / 2) + nextPos;
            nextMax = new Vector2(pageSize.x / 2, pageSize.y / 2) + nextPos;
        }
        else if (animationActive != -1)
        {
            animationActive = -1;

            ///Gets new corner positions.
            for (int i = 0; i < cardCorners.Length; i++)
            {
                float posX = Mathf.Sign(cardCorners[i].localScale.x) * (cardSize.x / 2) - cardCorners[i].localScale.x;
                float posY = Mathf.Sign(cardCorners[i].localScale.y) * (cardSize.y / 2) - cardCorners[i].localScale.y;

                nextCornerPos[i] = new Vector2(posX, posY);
            }

            ///Same concept as the last loop.
            for (int i = 0; i < cardEdges.Length; i++)
            {
                float posX = 0;
                float posY = 0;

                float scaleX = 0;
                float scaleY = 0;

                if (cardEdges[i].localPosition.x != 0)
                {
                    posX = Mathf.Sign(cardEdges[i].localPosition.x) * (cardSize.x / 2) - Mathf.Sign(cardEdges[i].localPosition.x) * (cardEdges[i].localScale.x / 2);
                    posY = 0;

                    scaleX = cornerSize;
                    scaleY = cardSize.y - cornerSize * 2;
                }
                else if (cardEdges[i].localPosition.y != 0)
                {
                    posX = 0;
                    posY = Mathf.Sign(cardEdges[i].localPosition.y) * (cardSize.y / 2) - Mathf.Sign(cardEdges[i].localPosition.y) * (cardEdges[i].localScale.y / 2);

                    scaleX = cardSize.x - cornerSize * 2;
                    scaleY = cornerSize;
                }

                nextEdgePos[i] = new Vector2(posX, posY);
                nextEdgeScale[i] = new Vector2(scaleX, scaleY);
            }

            nextCenterScale = cardSize - new Vector2(cornerSize * 2, cornerSize * 2);
            nextPos = cardPosition;

            nextSuperness = cardSuperness;

            nextMin = new Vector2(-cardSize.x / 2, -cardSize.y / 2) + nextPos;
            nextMax = new Vector2(cardSize.x / 2, cardSize.y / 2) + nextPos;
        }
    }
}
}