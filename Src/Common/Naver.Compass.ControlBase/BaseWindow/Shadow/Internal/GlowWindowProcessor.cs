using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Naver.Compass.Common.Win32;

namespace Naver.Compass.Common.Shadow
{
	/// <summary>
	/// <see cref="GlowWindow"/> count position
	/// </summary>
	internal abstract class GlowWindowProcessor
	{
		#region static members

		
		public static double GlowSize { get; set; }

		
		public static double EdgeSize { get; set; }

		static GlowWindowProcessor()
		{
           
            GlowSize =13.0;
			EdgeSize = 24.0;
		}

		#endregion

		public abstract Orientation Orientation { get; }

		
		public abstract HorizontalAlignment HorizontalAlignment { get; }

		
		public abstract VerticalAlignment VerticalAlignment { get; }

		
		public abstract double GetLeft(double ownerLeft, double ownerWidth);

		
		public abstract double GetTop(double ownerTop, double ownerHeight);

		
		public abstract double GetWidth(double ownerLeft, double ownerWidth);

		public abstract double GetHeight(double ownerTop, double ownerHeight);

		
		public abstract HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight);

		
		public abstract Cursor GetCursor(Point point, double actualWidht, double actualHeight);
	}
}
