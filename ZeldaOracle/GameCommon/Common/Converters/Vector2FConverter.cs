﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Converters {
	public class Vector2FConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context,
			Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo culture, object value)
		{
			if (value is string) {
				try {
					return Vector2F.Parse((string) value);
				}
				catch { }
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string)) {
				Vector2F vector = (Vector2F) value;
				return vector.X + "," + vector.Y;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
