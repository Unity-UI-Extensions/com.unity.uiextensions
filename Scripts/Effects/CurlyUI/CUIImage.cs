/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Effects/Extensions/Curly UI Image")]
    public class CUIImage : CUIGraphic
    {
        #region Nature

        public static int SlicedImageCornerRefVertexIdx = 2;
        public static int FilledImageCornerRefVertexIdx = 0;

        /// <summary>
        /// For sliced and filled image only.
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static int ImageTypeCornerRefVertexIdx(Image.Type _type)
        {
            if (_type == Image.Type.Sliced)
            {
                return SlicedImageCornerRefVertexIdx;
            }
            else
            {
                return FilledImageCornerRefVertexIdx;
            }
        }
        #endregion

        #region Description

        [Tooltip("For changing the size of the corner for tiled or sliced Image")]
        [HideInInspector]
        [SerializeField]
        public Vector2 cornerPosRatio = Vector2.one * -1; // -1 means unset value

        [HideInInspector]
        [SerializeField]
        protected Vector2 oriCornerPosRatio = Vector2.one * -1;
        public Vector2 OriCornerPosRatio
        {
            get
            {
                return oriCornerPosRatio;
            }
        }

        #endregion

        public Image UIImage
        {
            get
            {
                return (Image)uiGraphic;
            }
        }

        #region Configurations

        public override void ReportSet()
        {

            if (uiGraphic == null)
                uiGraphic = GetComponent<Image>();

            base.ReportSet();
        }

        protected override void modifyVertices(List<UIVertex> _verts)
        {
            if (!IsActive())
                return;

            if (UIImage.type == Image.Type.Filled)
            {
                Debug.LogWarning("Might not work well Radial Filled at the moment!");

            }
            else if (UIImage.type == Image.Type.Sliced || UIImage.type == Image.Type.Tiled)
            {
                // setting the starting cornerRatio
                if (cornerPosRatio == Vector2.one * -1)
                {
                    cornerPosRatio = _verts[ImageTypeCornerRefVertexIdx(UIImage.type)].position;
                    cornerPosRatio.x = (cornerPosRatio.x + rectTrans.pivot.x * rectTrans.rect.width) / rectTrans.rect.width;
                    cornerPosRatio.y = (cornerPosRatio.y + rectTrans.pivot.y * rectTrans.rect.height) / rectTrans.rect.height;

                    oriCornerPosRatio = cornerPosRatio;

                }

                // constraing the corner ratio 
                if (cornerPosRatio.x < 0)
                {
                    cornerPosRatio.x = 0;
                }
                if (cornerPosRatio.x >= 0.5f)
                {
                    cornerPosRatio.x = 0.5f;
                }
                if (cornerPosRatio.y < 0)
                {
                    cornerPosRatio.y = 0;
                }
                if (cornerPosRatio.y >= 0.5f)
                {
                    cornerPosRatio.y = 0.5f;
                }

                for (int index = 0; index < _verts.Count; index++)
                {
                    var uiVertex = _verts[index];

                    // finding the horizontal ratio position (0.0 - 1.0) of a vertex
                    float horRatio = (uiVertex.position.x + rectTrans.rect.width * rectTrans.pivot.x) / rectTrans.rect.width;
                    float verRatio = (uiVertex.position.y + rectTrans.rect.height * rectTrans.pivot.y) / rectTrans.rect.height;

                    if (horRatio < oriCornerPosRatio.x)
                    {
                        horRatio = Mathf.Lerp(0, cornerPosRatio.x, horRatio / oriCornerPosRatio.x);
                    }
                    else if (horRatio > 1 - oriCornerPosRatio.x)
                    {
                        horRatio = Mathf.Lerp(1 - cornerPosRatio.x, 1, (horRatio - (1 - oriCornerPosRatio.x)) / oriCornerPosRatio.x);
                    }
                    else
                    {
                        horRatio = Mathf.Lerp(cornerPosRatio.x, 1 - cornerPosRatio.x, (horRatio - oriCornerPosRatio.x) / (1 - oriCornerPosRatio.x * 2));
                    }

                    if (verRatio < oriCornerPosRatio.y)
                    {
                        verRatio = Mathf.Lerp(0, cornerPosRatio.y, verRatio / oriCornerPosRatio.y);
                    }
                    else if (verRatio > 1 - oriCornerPosRatio.y)
                    {
                        verRatio = Mathf.Lerp(1 - cornerPosRatio.y, 1, (verRatio - (1 - oriCornerPosRatio.y)) / oriCornerPosRatio.y);
                    }
                    else
                    {
                        verRatio = Mathf.Lerp(cornerPosRatio.y, 1 - cornerPosRatio.y, (verRatio - oriCornerPosRatio.y) / (1 - oriCornerPosRatio.y * 2));
                    }

                    uiVertex.position.x = horRatio * rectTrans.rect.width - rectTrans.rect.width * rectTrans.pivot.x;
                    uiVertex.position.y = verRatio * rectTrans.rect.height - rectTrans.rect.height * rectTrans.pivot.y;
                    //uiVertex.position.z = pos.z;

                    _verts[index] = uiVertex;
                }
            }

            base.modifyVertices(_verts);

        }

        #endregion

    }
}