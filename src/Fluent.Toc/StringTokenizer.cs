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

namespace Fluent.Text {

	internal class StringTokenizer {

		private string str;
		private char[] seperators;
		private bool returnSeperators;
		private int index;
		private bool hasMoreTokens;

		public bool HasMoreTokens {
			get { return hasMoreTokens; }
		}

		/// <summary>
		///  Constructs a StringTokenizer.
		/// </summary>
		/// <param name="str">The string to be tokenized.</param>
		public StringTokenizer(string str) : this(str, false, ' ') {
		}

		/// <summary>
		///  Constructs a StringTokenizer. 
		/// </summary>
		/// <param name="str">The string to be tokenized.</param>
		/// <param name="seperators">The seperators between tokens.</param>
		public StringTokenizer(string str, params char[] seperators) : this(str, false, seperators) {
			
		}

		/// <summary>
		/// Constructs a StringTokenizer. 
		/// </summary>
		/// <param name="str">The string to be tokenized.</param>
		/// <param name="returnSeperators">Whether to return sperators as tokens.</param>
		/// <param name="seperators">The seperators between tokens.</param>
		public StringTokenizer(string str, bool returnSeperators, params char[] seperators){

			this.str = str;
			this.returnSeperators = returnSeperators;
			this.seperators = seperators;

			if(str != null && str.Length > 0 ){
				hasMoreTokens = true;
			}
		}

		/// <summary>
		/// Returns the next token.
		/// </summary>
		public string ReadToken(){
			if(!HasMoreTokens){
				throw new InvalidOperationException("StringTokenizer has no more tokens.");
			}

			string token;
			int nextIndex;

			nextIndex = str.IndexOfAny(seperators, index);
			
			if(nextIndex < 0){
				token = str.Substring(index);
				hasMoreTokens = false;
			} else {
				token = str.Substring(index, nextIndex - index);
			}

			index = nextIndex + 1;

			return token;
		}

		/// <summary>
		/// Returns the remaining string.
		/// </summary>
		public string ReadToEnd(){
			if(!HasMoreTokens){
				throw new InvalidOperationException("StringTokenizer has no more tokens.");
			}

			hasMoreTokens = false;
			return str.Substring(index);
		}

		/// <summary>
		/// Returns the remaining string.
		/// </summary>
		public char ReadChar(){
			if(!HasMoreTokens){
				throw new InvalidOperationException("StringTokenizer has no more tokens.");
			}
			char c = str[index];
			index++;
			if(str.Length <= index){
				hasMoreTokens = false;
			}
		
			return c;
		}

		/// <summary>
		/// Calculates the remaining number of tokens.
		/// </summary>
		/// <returns></returns>
		public int CountTokens(){
			if(HasMoreTokens) {
				int count = 0, nextIndex = index;
				do {
					nextIndex = str.IndexOfAny(seperators, nextIndex) + 1;
					count++;
				} while (nextIndex > 0);
				return count;
			} else {
				return 0;
			}
		}
	}
}
