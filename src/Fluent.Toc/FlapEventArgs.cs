/*
 *  Fluent.Toc is a .NET component for communicating with 
 *  AOL's Instant Messenger (AIM) service. 
 * 
 *  Copyright 2004 by Fluent Consulting
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;

namespace Fluent.Toc {

	/// <summary>
	/// Provides data for the Flap event.
	/// </summary>
	public class FlapEventArgs : EventArgs {

		private string flap;

		/// <summary>
		/// The raw flap data.
		/// </summary>
		public string Flap {
			get{ return flap; }
		}

		/// <summary>
		/// Constructor for FlapEventArgs.
		/// </summary>
		/// <param name="flap">The raw flap data.</param>
		protected internal FlapEventArgs(string flap) {
			this.flap = flap;
		}
	}
}
