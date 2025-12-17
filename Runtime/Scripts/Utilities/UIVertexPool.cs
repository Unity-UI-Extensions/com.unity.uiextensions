///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	public class UIVertexPool
	{
		Stack<UIVertex[]> _pool;
		List<UIVertex[]> _used;

		public UIVertexPool(int size)
		{
			_pool = new Stack<UIVertex[]>(size);
			_used = new List<UIVertex[]>();

			for (int i = 0; i < size; i++)
			{
				_pool.Push(new UIVertex[4]);
			}
		}

		public UIVertex[] Get()
		{
			UIVertex[] item = null;
			if (_pool.Count > 0)
			{
				item = _pool.Pop();
			}
			else
			{
				item = new UIVertex[4];
				for (int i = 0; i < 4; i++)
				{
					item[i] = UIVertex.simpleVert;
				}
			}

			_used.Add(item);
			return item;
		}

		public void ReleaseAll()
		{
			int count = _used.Count;
			for (int i = 0; i < count; i++)
			{
				Release(_used[i]);
			}
			_used.Clear();
		}

		public void Release(UIVertex[] vrtx)
		{
			if (vrtx != null)
			{
				_pool.Push(vrtx);
			}
		}

		public void Clear()
		{
			_pool.Clear();
		}
	}
}