///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

namespace UnityEngine.UI.Extensions
{
	/// <summary>
	/// Image fill type controls how to display the image.
	/// </summary>
	public enum ImageFillType
	{
		/// <summary>
		/// Displays the full Image
		/// </summary>
		/// <remarks>
		/// This setting shows the entire image stretched across the Image's RectTransform
		/// </remarks>
		Simple = 0,

		/// <summary>
		/// Displays the Image as a 9-sliced graphic.
		/// </summary>
		/// <remarks>
		/// A 9-sliced image displays a central area stretched across the image surrounded by a border comprising of 4 corners and 4 stretched edges.
		///
		/// This has the effect of creating a resizable skinned rectangular element suitable for dialog boxes, windows, and general UI elements.
		///
		/// Note: For this method to work properly the Sprite assigned to Image.sprite needs to have Sprite.border defined.
		/// </remarks>
		Sliced = 1,

		/// <summary>
		/// Displays a sliced Sprite with its resizable sections tiled instead of stretched.
		/// </summary>
		/// <remarks>
		/// A Tiled image behaves similarly to a UI.Image.Type.Sliced|Sliced image, except that the resizable sections of the image are repeated instead of being stretched. This can be useful for detailed UI graphics that do not look good when stretched.
		///
		/// It uses the Sprite.border value to determine how each part (border and center) should be tiled.
		///
		/// The Image sections will repeat the corresponding section in the Sprite until the whole section is filled. The corner sections will be unaffected and will draw in the same way as a Sliced Image. The edges will repeat along their lengths. The center section will repeat across the whole central part of the Image.
		///
		/// The Image section will repeat the corresponding section in the Sprite until the whole section is filled.
		///
		/// Be aware that if you are tiling a Sprite with borders or a packed sprite, a mesh will be generated to create the tiles. The size of the mesh will be limited to 16250 quads; if your tiling would require more tiles, the size of the tiles will be enlarged to ensure that the number of generated quads stays below this limit.
		///
		/// For optimum efficiency, use a Sprite with no borders and with no packing, and make sure the Sprite.texture wrap mode is set to TextureWrapMode.Repeat.These settings will prevent the generation of additional geometry.If this is not possible, limit the number of tiles in your Image.
		/// </remarks>
		Tiled = 2,

		/// <summary>
		/// UNSUPPORTED
		/// Displays only a portion of the Image.
		/// </summary>
		/// <remarks>
		/// A Filled Image will display a section of the Sprite, with the rest of the RectTransform left transparent. The Image.fillAmount determines how much of the Image to show, and Image.fillMethod controls the shape in which the Image will be cut.
		///
		/// This can be used for example to display circular or linear status information such as timers, health bars, and loading bars.
		/// </remarks>
		//Filled = 3
	}
}