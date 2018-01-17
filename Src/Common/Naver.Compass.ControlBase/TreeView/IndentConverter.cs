using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Naver.Compass.Module
{
	/// <summary>
	/// 计算 <see cref="System.Windows.Controls.TreeViewItem"/> 的缩进的转换器。
	/// </summary>
	[ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
	public sealed class IndentConverter : IValueConverter
	{
		/// <summary>
		/// 获取或设置缩进的像素个数。
		/// </summary>
		public double Indent { get; set; }
		/// <summary>
		/// 获取或设置初始的左边距。
		/// </summary>
		public double MarginLeft { get; set; }
		/// <summary>
		/// 转换值。
		/// </summary>
		/// <param name="value">绑定源生成的值。</param>
		/// <param name="targetType">绑定目标属性的类型。</param>
		/// <param name="parameter">要使用的转换器参数。</param>
		/// <param name="culture">要用在转换器中的区域性。</param>
		/// <returns>转换后的值。如果该方法返回 <c>null</c>，则使用有效的 <c>null</c> 值。</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TreeViewItem item = value as TreeViewItem;
			if (item == null)
			{
				return new Thickness(0);
			}
			return new Thickness(this.MarginLeft + this.Indent * item.GetDepth(), 0, 0, 0);
		}
		/// <summary>
		/// 转换值。
		/// </summary>
		/// <param name="value">绑定目标生成的值。</param>
		/// <param name="targetType">要转换到的类型。</param>
		/// <param name="parameter">要使用的转换器参数。</param>
		/// <param name="culture">要用在转换器中的区域性。</param>
		/// <returns>转换后的值。如果该方法返回 <c>null</c>，则使用有效的 <c>null</c> 值。</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
