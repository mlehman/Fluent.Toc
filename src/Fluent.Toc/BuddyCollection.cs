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
using System.Collections;

namespace Fluent.Toc {
	
	/// <summary>
	/// BuddyCollection.
	/// </summary>
	public class BuddyInfoCollection: System.Collections.CollectionBase {

		private Hashtable lookUp;
		private TocClient tocClient;

		
		private class BuddyGroupComparer : IComparer  {
			int IComparer.Compare( Object x, Object y )  {
				return ((BuddyInfo)x).Group.CompareTo(((BuddyInfo)y).Group);
			}
		}

		internal BuddyInfoCollection(TocClient tocClient) {
			this.tocClient = tocClient;
			lookUp = new Hashtable();
		}


		/// <summary>
		/// Whether the collection contains a buddy.
		/// </summary>
		public bool Contains(BuddyInfo buddyInfo){
			return lookUp.Contains(buddyInfo.ScreenName.ToLower());
		}

	
		/// <summary>
		/// Add a Buddy to the collection.
		/// </summary>
		/// <returns>Index of Buddy. Returns -1 if Buddy is already in collection.</returns>
		public int Add(BuddyInfo buddyInfo) {
			string key = buddyInfo.ScreenName.ToLower();
			if(lookUp.ContainsKey(key)){
				return -1;
			} else {

				lookUp.Add(key, buddyInfo);
				int index = List.Add(buddyInfo);

				if(tocClient.Connected){
					tocClient.AddBuddy(buddyInfo.ScreenName);
				}

				return index;
			}
		}
		
		/// <summary>
		/// Remove a Buddy to the collection.
		/// </summary>
		public void Remove(BuddyInfo buddyInfo) {
			if(tocClient.Connected){
				tocClient.RemoveBuddy(buddyInfo.ScreenName);
			}
			lookUp.Remove(buddyInfo.screenName.ToLower());
			List.Remove(buddyInfo);
		}
		
		/// <summary>
		/// Get a buddy from the collection by index.
		/// </summary>
		public BuddyInfo this[int index] {
			get {return (BuddyInfo)List[index];}
		}

		/// <summary>
		/// Get a buddy from the collection by screen name.
		/// </summary>
		public BuddyInfo this[string screenName] {
			get { return (BuddyInfo)lookUp[screenName.ToLower()];}
		}

		/// <summary>
		/// Copy the buddies to an array.
		/// </summary>
		public void CopyTo(BuddyInfo[] array, int index) {
			List.CopyTo(array, index);
		}

		/// <summary>
		/// Insert a buddy into the collection.
		/// </summary>
		public void Insert(int index, BuddyInfo value) {
			List.Insert( index, value);
		}

		/// <summary>
		/// Get the index of buddy from the collection.
		/// </summary>
		public int IndexOf(BuddyInfo value) {
			return List.IndexOf(value);
		}

		protected internal BuddyInfo[] SortBuddyList(){
			BuddyInfo[] buddies = new BuddyInfo[this.Count];
			this.List.CopyTo(buddies,0);
			Array.Sort(buddies, new BuddyGroupComparer());
			return buddies;
		}

	}

}
