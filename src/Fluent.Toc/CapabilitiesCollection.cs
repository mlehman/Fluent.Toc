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
	/// The CapabilityCollection is a collection of Capabilites that are supported by the client.
	/// </summary>
	/// <remarks>
	///  You can add Capabilites to the collection or remove Capabilites from the collection.
	/// </remarks>
	public class CapabilityCollection: System.Collections.CollectionBase {

		/// <summary>
		/// Default constructor
		/// </summary>
		public CapabilityCollection() {
		}
	
		/// <summary>
		/// Add a capability to the collection.
		/// </summary>
		public int Add(Capability value) {
			return List.Add(value);
		}
		
		/// <summary>
		/// Remove a capability from the collection.
		/// </summary>
		public void Remove(Capability value) {
			List.Remove(value);
		}
		
		/// <summary>
		/// Get a capability from the collection.
		/// </summary>
		public Capability this[int index] {
			get {return (Capability)List[index];}
			set { List[index] = value; }
		}

		/// <summary>
		/// Copy the capabilities to an array.
		/// </summary>
		public void CopyTo(Capability[] array, int index) {
			List.CopyTo(array, index);
		}

		/// <summary>
		/// Insert a capability into the collection.
		/// </summary>
		public void Insert(int index, Capability value) {
			List.Insert( index, value);
		}

		/// <summary>
		/// Get the index of capability from the collection.
		/// </summary>
		public int IndexOf(Capability value) {
			return List.IndexOf(value);
		}

		/// <summary>
		/// Whether the collection contains a capability.
		/// </summary>
		public bool Contains(Capability value){
			return List.Contains(value);
		}

	}
}
