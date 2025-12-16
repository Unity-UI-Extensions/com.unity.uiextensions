///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-circle-segmented

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    public class UIVertexPool
	{
        private Stack<UIVertex[]> _pool;

        public UIVertexPool(int size)
		{
            _pool = new Stack<UIVertex[]>(size);

			for (int i = 0; i < size; i++)
			{
				_pool.Push(new UIVertex[4]);
			}
		}

		public UIVertex[] Get()
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop();
			}
			return new UIVertex[4];
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
