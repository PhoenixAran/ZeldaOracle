﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Tiles {
	
	public interface IColoredTile {
		PuzzleColor Color { get; }
	}
}
