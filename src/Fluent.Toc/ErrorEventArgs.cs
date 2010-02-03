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
	/// Provides data for the Error event.
	/// </summary>
	public class ErrorEventArgs : EventArgs {

		public const int GeneralUserNotAvailable = 901;
		public const int GeneralWarningNotAvailable = 902;
		public const int GeneralMessageDropped = 903;

		private int code;
		private string message;
		private string arg;

		/// <summary>
		/// Gets the error code associated with this error.
		/// </summary>
		public int Code {
			get{ return code; }
		}

		/// <summary>
		/// Gets a message that describes the current error.
		/// </summary>
		public string Message {
			get{ return message; }
		}

		/// <summary>
		/// Gets the argument that is associated with the current error.
		/// </summary>
		public string Arg {
			get{ return arg; }
		}

		protected internal ErrorEventArgs(int code, string message, string arg) {
			this.code = code;
			this.message = message;
			this.arg = arg;
		}
	}
}
