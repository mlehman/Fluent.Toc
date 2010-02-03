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
	/// Holds information about a buddy.
	/// </summary>
	public class BuddyInfo {

		internal string screenName;
		internal string group;
		internal bool online;
		internal int evilAmount;
		internal DateTime signOnTime;
		internal TimeSpan idleTime;
		internal bool isOnAol;
		internal BuddyUserClass userClass;
		internal bool isAvailable;

		/// <summary>
		/// The screen name of the buddy.
		/// </summary>
		public string ScreenName {
			get { return screenName; }
		}

		/// <summary>
		/// The group of the buddy.
		/// </summary>
		public string Group {
			get { return group; }
		}

		/// <summary>
		/// Whether the buddy is online.
		/// </summary>
		public bool Online {
			get { return online; }
		}

		/// <summary>
		/// A percentage.
		/// </summary>
		public int EvilAmount {
			get { return evilAmount; }
		}

		/// <summary>
		/// The date/time the buddy signed on.
		/// </summary>
		public DateTime SignOnTime {
			get { return signOnTime; }
		}

		/// <summary>
		/// The length of time the buddy has been idle.
		/// </summary>
		public TimeSpan IdleTime {
			get { return idleTime; }
		}

		/// <summary>
		/// If the buddy is on AOL.
		/// </summary>
		public bool IsOnAol {
			get { return isOnAol; }
		}

		/// <summary>
		/// The user class of the buddy.
		/// </summary>
		public BuddyUserClass UserClass {
			get { return userClass; } 
		}

		/// <summary>
		/// Whether the buddy is available.
		/// </summary>
		public bool IsAvailable {
			get { return isAvailable; }
		}

		public BuddyInfo() {
		}

		public BuddyInfo(string screenName){
			this.screenName = screenName;
		}

		public BuddyInfo(string screenName, string group){
			this.screenName = screenName;
			this.group = group;
		}
	}
}
