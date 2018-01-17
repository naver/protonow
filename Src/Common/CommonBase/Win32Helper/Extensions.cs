using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Naver.Compass.Common.Win32
{
	public static class Extensions
	{
		/// <summary>
		///  <see cref="Visual"/> 
		/// </summary>
		/// <returns>
		/// DPI construct
		/// </returns>
		public static Dpi? GetSystemDpi(this Visual visual)
		{
			var source = PresentationSource.FromVisual(visual);
			if (source != null && source.CompositionTarget != null)
			{
				return new Dpi(
					(uint)(Dpi.Default.X * source.CompositionTarget.TransformToDevice.M11),
					(uint)(Dpi.Default.Y * source.CompositionTarget.TransformToDevice.M22));
			}

			return null;
		}
	}
}
