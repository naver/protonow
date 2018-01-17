using System.Windows.Controls;

namespace Naver.Compass.Module
{
	/// <summary>
	/// <see cref="System.Windows.Controls.TreeViewItem"/> 的扩展方法。
	/// </summary>
	public static class TreeViewItemExt
	{
		/// <summary>
		/// 返回指定 <see cref="System.Windows.Controls.TreeViewItem"/> 的深度。
		/// </summary>
		/// <param name="item">要获取深度的 <see cref="System.Windows.Controls.TreeViewItem"/> 对象。</param>
		/// <returns><see cref="System.Windows.Controls.TreeViewItem"/> 所在的深度。</returns>
		public static int GetDepth(this TreeViewItem item)
		{
			int depth = 0;
			while ((item = item.GetAncestor<TreeViewItem>()) != null)
			{
				depth++;
			}
			return depth;
		}
	}
}
