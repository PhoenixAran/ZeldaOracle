﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using ZeldaOracle.Common.Converters;

namespace ZeldaOracle.Common.Geometry {

	/// <summary>Struct used to represent an axis-aligned or diagonal angle.</summary>
	[Serializable]
	[TypeConverter(typeof(AngleConverter))]
	public struct Angle : IConvertible {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>the angle pointing right (east).</summary>
		public static readonly Angle Right = new Angle(0);
		/// <summary>the angle pointing up-right (northeast).</summary>
		public static readonly Angle UpRight = new Angle(1);
		/// <summary>the angle pointing up (north).</summary>
		public static readonly Angle Up = new Angle(2);
		/// <summary>the angle pointing up-left (northwest).</summary>
		public static readonly Angle UpLeft = new Angle(3);
		/// <summary>the angle pointing left (west).</summary>
		public static readonly Angle Left = new Angle(4);
		/// <summary>the angle pointing down-left (southwest).</summary>
		public static readonly Angle DownLeft = new Angle(5);
		/// <summary>the angle pointing down (south).</summary>
		public static readonly Angle Down = new Angle(6);
		/// <summary>the angle pointing down-left (southeast).</summary>
		public static readonly Angle DownRight = new Angle(7);

		/// <summary>the angle pointing right (east).</summary>
		public static readonly Angle East = new Angle(0);
		/// <summary>the angle pointing up-right (northeast).</summary>
		public static readonly Angle NorthEast = new Angle(1);
		/// <summary>the angle pointing up (north).</summary>
		public static readonly Angle North = new Angle(2);
		/// <summary>the angle pointing up-left (northwest).</summary>
		public static readonly Angle NorthWest = new Angle(3);
		/// <summary>the angle pointing left (west).</summary>
		public static readonly Angle West = new Angle(4);
		/// <summary>the angle pointing down-left (southwest).</summary>
		public static readonly Angle SouthWest = new Angle(5);
		/// <summary>the angle pointing down (south).</summary>
		public static readonly Angle South = new Angle(6);
		/// <summary>the angle pointing down-left (southeast).</summary>
		public static readonly Angle SouthEast = new Angle(7);
		
		/// <summary>An invalid angle.</summary>
		public static readonly Angle Invalid = new Angle(-1);

		/// <summary>The total number of unique angles.</summary>
		public const int Count = 8;

		/// <summary>Iterate all eight angles.</summary>
		public static IEnumerable<Angle> Range {
			get {
				for (int index = 0; index < Angle.Count; index++)
					yield return new Angle(index);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The index of the angle, from 0 to 7. 0 is right, 1 is up-right, 2
		/// is up, etc.. Angles increase counter-clockwise. If this value is not
		/// between 0 and 7, then the angle is considered invalid.</summary>
		private int index;
		

		//-----------------------------------------------------------------------------
		// Constructors / Factory Functions
		//-----------------------------------------------------------------------------

		/// <summary>Create an angle from an index (0 to 3).</summary>
		public Angle(int index) {
			this.index = index;
		}

		/// <summary>Return the nearest angle from a vector.</summary>
		public static Angle FromVector(Vector2F vector) {
			return FromRadians(GMath.Atan2(-vector.Y, vector.X));
		}

		/// <summary>Return the nearest angle from a point.</summary>
		public static Angle FromPoint(Point2I point) {
			if (point.IsZero)
				return Angle.Invalid;
			else if (point.Y == 0) {
				// The point is horizontal
				if (point.X > 0)
					return Angle.Right;
				else
					return Angle.Left;
			}
			else if (point.X == 0) {
				// The point is vertical
				if (point.Y > 0)
					return Angle.Down;
				else
					return Angle.Up;
			}
			else if (GMath.Abs(point.X) == GMath.Abs(point.Y)) {
				// The point is perfectly diagonal
				if (point.X > 0) {
					if (point.Y > 0)
						return Angle.DownRight;
					else
						return Angle.UpRight;
				}
				else {
					if (point.Y > 0)
						return Angle.DownLeft;
					else
						return Angle.UpLeft;
				}
			}
			else {
				// Calculate the point's angle in radians then convert to an angle
				// struct
				return FromRadians(GMath.Atan2((float) -point.Y, (float) point.X));
			}
		}
		
		/// <summary>Return the nearest angle from an angle in radians.</summary>
		public static Angle FromRadians(float radians) {
			return new Angle(GMath.Wrap(
				GMath.RoundI((radians * 4.0f) / GMath.Pi), 8));
		}
		

		//-----------------------------------------------------------------------------
		// Unary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Return the opposite angle.</summary>
		public Angle Reverse() {
			return new Angle((index + 4) % 8);
		}

		/// <summary>Return the angle flipped horizontally over the y-axis.
		/// </summary>
		public Angle FlipHorizontal() {
			return new Angle((12 - index) % 8);
		}
		
		/// <summary>Return the angle flipped vertically over the x-axis.
		/// </summary>
		public Angle FlipVertical() {
			return new Angle((8 - index) % 8);
		}

		
		//-----------------------------------------------------------------------------
		// Static Unary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Return the angle as is (this operator does nothing).</summary>
		public static Angle operator +(Angle a) {
			return a;
		}

		/// <summary>Return the opposite angle.</summary>
		public static Angle operator -(Angle a) {
			return a.Reverse();
		}

		/// <summary>Rotate the angle once, clockwise.</summary>
		public static Angle operator ++(Angle a) {
			return new Angle((a.index + 1) % 8);
		}

		/// <summary>Rotate the angle once, counter-clockwise.</summary>
		public static Angle operator --(Angle a) {
			return new Angle((a.index + 7) % 8);
		}
		
		
		//-----------------------------------------------------------------------------
		// Binary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Rotate the angle either clockwise or counter-clockwise.</summary>
		public Angle Rotate(int amount,
			WindingOrder windingOrder = WindingOrder.CounterClockwise)
		{
			if (windingOrder == WindingOrder.Clockwise)
				amount = -amount;
			return new Angle(GMath.Wrap(index + amount, 8));
		}
		
		/// <summary>Return the nearest distance between this angle and another. A
		/// positive distance is counter-clockwise while a negative distance is
		/// clockwise.</summary>
		public int NearestDistanceTo(Angle other) {
			return Angle.NearestDistance(this, other);
		}
		
		/// <summary>Return the nearest distance between this angle and another.
		/// This will return a positive distance, and set the winding order output
		/// variable to direction-of-rotation to rotate that distance to get to the
		/// destination angle.</summary>
		public int NearestDistanceTo(Angle other, out WindingOrder windingOrder) {
			int signedDistance = Angle.NearestDistance(this, other);
			if (signedDistance < 0) {
				windingOrder = WindingOrder.Clockwise;
				return -signedDistance;
			}
			else {
				windingOrder = WindingOrder.CounterClockwise;
				return signedDistance;
			}
		}
		
		/// <summary>Return the distance from one angle to another when traveling in
		/// the given winding order. The result will always be positive.</summary>
		public int DistanceTo(Angle other, WindingOrder windingOrder) {
			return Angle.Distance(this, other, windingOrder);
		}
		

		//-----------------------------------------------------------------------------
		// Static Binary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Rotate the angle counter-clockwise by the given amount.
		/// </summary>
		public static Angle operator +(Angle angle, int amount) {
			return angle.Rotate(amount, WindingOrder.CounterClockwise);
		}
		
		/// <summary>Rotate the angle clockwise by the given amount.</summary>
		public static Angle operator -(Angle angle, int amount) {
			return angle.Rotate(amount, WindingOrder.Clockwise);
		}
		
		/// <summary>Return the nearest distance between two angles. A positive
		/// distance is counter-clockwise while a negative distance is clockwise.
		/// </summary>
		public static int NearestDistance(Angle from, Angle to) {
			int clockwiseDistance = Angle.Distance(
				from, to, WindingOrder.Clockwise);
			int counterClockwiseDistance = Angle.Distance(
				from, to, WindingOrder.CounterClockwise);
			if (clockwiseDistance < counterClockwiseDistance)
				return -clockwiseDistance;
			else
				return counterClockwiseDistance;
		}

		/// <summary>Return the distance from one angle to another when traveling in
		/// the given winding order. The result will always be positive.</summary>
		public static int Distance(Angle from, Angle to,
			WindingOrder windingOrder)
		{
			if (windingOrder == WindingOrder.Clockwise) {
				if (to.index > from.index)
					return (8 + from.index - to.index) % 8;
				else
					return (from.index - to.index);
			}
			else {
				if (from.index > to.index)
					return (8 + to.index - from.index) % 8;
				else
					return (to.index - from.index);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Implicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Convert an integer to an angle.</summary>
		public static implicit operator Angle(int index) {
			return new Angle(index);
		}

		/// <summary>Convert an angle to an integer.</summary>
		public static implicit operator int(Angle angle) {
			return angle.index;
		}

		/// <summary>Return the string representation of the angle (right, up,
		/// left, or down).</summary>
		public override string ToString() {
			if (index == 0)
				return "east";
			else if (index == 1)
				return "northeast";
			else if (index == 2)
				return "north";
			else if (index == 3)
				return "northwest";
			else if (index == 4)
				return "west";
			else if (index == 5)
				return "southwest";
			else if (index == 6)
				return "south";
			else if (index == 7)
				return "southeast";
			return "invalid";
		}

		
		//-----------------------------------------------------------------------------
		// Explicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Return the angle's represented as a direction. If the angle is not
		/// axis-aligned, then this returns Direction.Invalid.</summary>
		public Direction ToDirection() {
			if (IsAxisAligned)
				return new Direction(index / 2);
			else
				return Direction.Invalid;
		}

		/// <summary>Return the angle's angle in radians.</summary>
		public float ToRadians() {
			return (index * GMath.QuarterPi);
		}

		/// <summary>Return the angle as a polar vector with the given magnitude.
		/// </summary>
		public Vector2F ToVector(float magnitude = 1.0f) {
			float radians = ToRadians();
			return new Vector2F(GMath.Cos(radians), -GMath.Sin(radians)) * magnitude;
		}

		/// <summary>Return the angle as a point with the given magnitude.
		/// </summary>
		public Point2I ToPoint(int magnitude = 1) {
			if (index == 0)
				return new Point2I(magnitude, 0); // right
			else if (index == 1)
				return new Point2I(magnitude, -magnitude); // up-right
			else if (index == 2)
				return new Point2I(0, -magnitude); // up
			else if (index == 3)
				return new Point2I(-magnitude, -magnitude); // up-left
			else if (index == 4)
				return new Point2I(-magnitude, 0); // left
			else if (index == 5)
				return new Point2I(-magnitude, magnitude); // down-left
			else if (index == 6)
				return new Point2I(0, magnitude); // down
			else if (index == 7)
				return new Point2I(magnitude, magnitude); // down-right
			else
				return Point2I.Zero; // invalid
		}
		
		/// <summary>Try to parse an angle from a string. Returns true if the parse
		/// was successful.</summary>
		public static bool TryParse(string value, bool ignoreCase,
			out Angle result)
		{
			if (ignoreCase)
				value = value.ToLower();
			if (value == "right" || value == "east")
				result = Angle.Right;
			else if (value == "left" || value == "west")
				result = Angle.Left;
			else if (value == "up" || value == "north")
				result = Angle.Up;
			else if (value == "down" || value == "south")
				result = Angle.Down;
			else if (value == "upright" || value == "rightup" || value == "northeast")
				result = Angle.NorthEast;
			else if (value == "upleft" || value == "leftup" || value == "northwest")
				result = Angle.NorthWest;
			else if (value == "downleft" || value == "leftdown" || value == "southwest")
				result = Angle.SouthWest;
			else if (value == "downright" || value == "rightdown" || value == "southeast")
				result = Angle.SouthEast;
			else {
				result = Angle.Invalid;
				return false;
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Return the index of the angle (0 to 7).</summary>
		public int Index {
			get { return index; }
		}

		/// <summary>Return true if this is a valid angle.</summary>
		public bool IsValid {
			get { return (index >= 0 && index < 8); }
		}

		/// <summary>Return true if the angle is horizontal (left or right).
		/// </summary>
		public bool IsHorizontal {
			get { return (index == 0 || index == 4); }
		}

		/// <summary>Return true if the angle is vertical (up or down).</summary>
		public bool IsVertical {
			get { return (index == 2 || index == 6); }
		}

		/// <summary>Return true if the angle is axis-aligned.</summary>
		public bool IsAxisAligned {
			get { return (index % 2 == 0); }
		}


		//-----------------------------------------------------------------------------
		// IConvertable
		//-----------------------------------------------------------------------------

		TypeCode IConvertible.GetTypeCode() {
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		byte IConvertible.ToByte(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		char IConvertible.ToChar(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		double IConvertible.ToDouble(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		short IConvertible.ToInt16(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		int IConvertible.ToInt32(IFormatProvider provider) {
			return (int) this;
		}
		long IConvertible.ToInt64(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		float IConvertible.ToSingle(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		string IConvertible.ToString(IFormatProvider provider) {
			return ToString();
		}
		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == typeof(int))
				return (int) this;
			if (conversionType == typeof(Angle))
				return ToDirection();
			throw new InvalidCastException();
		}
		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		uint IConvertible.ToUInt32(IFormatProvider provider) {
			throw new InvalidCastException();
		}
		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			throw new InvalidCastException();
		}
	}
}
