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
	/// A Capability that a client supports. Capabilities are identified by their UUID.
	/// </summary>
	public struct Capability {

		private string uuid;
		private string name;

		private static Capability voice = new Capability("09461341-4C7F-11D1-8222-444553540000","Voice");
		private static Capability buddyIcon = new Capability("09461346-4C7F-11D1-8222-444553540000","BuddyIcon"); 
		private static Capability fileGet = new Capability("09461348-4C7F-11D1-8222-444553540000","FileGet"); 
		private static Capability fileSend = new Capability("09461343-4C7F-11D1-8222-444553540000","FileSend"); 
		private static Capability games = new Capability("0946134a-4C7F-11D1-8222-444553540000","Games"); 
		private static Capability image = new Capability("09461345-4C7F-11D1-8222-444553540000","Image"); 
		private static Capability stocks = new Capability("09461347-4C7F-11D1-8222-444553540000","Stocks"); 

		public static Capability Voice { get{ return voice;} }
		public static Capability BuddyIcon { get{ return buddyIcon;} }
		public static Capability FileGet { get{ return fileGet;} }
		public static Capability FileSend { get{ return fileSend;} }
		public static Capability Games { get{ return games;} }
		public static Capability Image { get{ return image;} }
		public static Capability Stocks { get{ return stocks;} }


		public Capability(string uuid, string name) {
			this.uuid = uuid;
			this.name = name;
		}

		/// <summary>
		/// The Universal Unique Identifier that identifies this capability.
		/// </summary>
		public string UUID {
			get { return uuid; }
		}

		/// <summary>
		/// Gets the name of this capability.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		/// Determines if the capabilities are the same.
		/// </summary>
		public static bool operator== (Capability c1, Capability c2) {
			return c1.UUID.Equals(c2.UUID);
		}
        
		/// <summary>
		/// Determines if the capabilities are the same.
		/// </summary>
		public static bool operator!= (Capability c1, Capability c2) {
			return !c1.UUID.Equals(c2.UUID);
		}

		/// <summary>
		/// Determines if the capabilities are the same.
		/// </summary>
		public override bool Equals(Object obj) {
			// Check for null values and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Capability c = (Capability)obj;
			return this.UUID == c.UUID;
		}

		public override int GetHashCode() {
			return this.UUID.GetHashCode();
		}


	}
}
