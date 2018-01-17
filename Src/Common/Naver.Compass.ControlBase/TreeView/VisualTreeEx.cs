using System.Windows;
using System.Windows.Media;

namespace Naver.Compass.Module
{
	/// <summary>
	/// WPF 可视化树的扩展方法。
	/// </summary>
	public static class VisualTreeEx
	{
		/// <summary>
		/// 返回指定对象的特定类型的祖先。
		/// </summary>
		/// <typeparam name="T">要获取的祖先的类型。</typeparam>
		/// <param name="source">获取的祖先，如果不存在则为 <c>null</c>。</param>
		/// <returns>获取的祖先对象。</returns>
		public static T GetAncestor<T>(this DependencyObject source)
			where T : DependencyObject
		{
			do
			{
				source = VisualTreeHelper.GetParent(source);
			} while (source != null && !(source is T));
			return source as T;
		}
	}
}
