/// Credit senritsu 
/// Sourced from - https://github.com/senritsu/unitility/blob/master/Assets/Unitility/GUI/RaycastMask.cs

/***************************************************************************\
The MIT License (MIT)

Copyright (c) 2014 Jonas Schiegl (https://github.com/senritsu)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
\***************************************************************************/


namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Extensions/Raycast Mask")]
    public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
    {
        private Image _image;
        private Sprite _sprite;

        void Start()
        {
            _image = GetComponent<Image>();
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            _sprite = _image.sprite;

            var rectTransform = (RectTransform)transform;
            Vector2 localPositionPivotRelative;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out localPositionPivotRelative);

            // convert to bottom-left origin coordinates
            var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
                localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);

            var spriteRect = _sprite.textureRect;
            var maskRect = rectTransform.rect;

            var x = 0;
            var y = 0;
            // convert to texture space
            switch (_image.type)
            {

                case Image.Type.Sliced:
                    {
                        var border = _sprite.border;
                        // x slicing
                        if (localPosition.x < border.x)
                        {
                            x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
                        }
                        else if (localPosition.x > maskRect.width - border.z)
                        {
                            x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
                        }
                        else
                        {
                            x = Mathf.FloorToInt(spriteRect.x + border.x +
                                                 ((localPosition.x - border.x) /
                                                 (maskRect.width - border.x - border.z)) *
                                                 (spriteRect.width - border.x - border.z));
                        }
                        // y slicing
                        if (localPosition.y < border.y)
                        {
                            y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
                        }
                        else if (localPosition.y > maskRect.height - border.w)
                        {
                            y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
                        }
                        else
                        {
                            y = Mathf.FloorToInt(spriteRect.y + border.y +
                                                 ((localPosition.y - border.y) /
                                                 (maskRect.height - border.y - border.w)) *
                                                 (spriteRect.height - border.y - border.w));
                        }
                    }
                    break;
                case Image.Type.Simple:
                default:
                    {
                        // conversion to uniform UV space
                        x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
                        y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
                    }
                    break;
            }

            // destroy component if texture import settings are wrong
            try
            {
                return _sprite.texture.GetPixel(x, y).a > 0;
            }
            catch (UnityException)
            {
                Debug.LogWarning("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
                Destroy(this);
                return false;
            }
        }
    }
}