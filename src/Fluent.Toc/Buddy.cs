using System;

namespace Fluent.Toc {

	/// <summary>
	/// Summary description for Buddy.
	/// </summary>
	public class Buddy {

		protected internal string screenName;
		protected internal string group;
		protected internal bool online;
		protected internal int evilAmount;
		protected internal DateTime signOnTime;
		protected internal TimeSpan idleTime;
		protected internal bool isOnAol;
		protected internal BuddyUserClass userClass;
		protected internal bool isAvailable;

		public string ScreenName {
			get { return screenName; }
		}

		public string Group {
			get { return group; }
		}

		public bool Online {
			get { return online; }
		}

		public int EvilAmount {
			get { return evilAmount; }
		}

		public DateTime SignOnTime {
			get { return signOnTime; }
		}

		public TimeSpan IdleTime {
			get { return idleTime; }
		}

		public bool IsOnAol {
			get { return isOnAol; }
		}

		public BuddyUserClass UserClass {
			get { return userClass; } 
		}

		public bool IsAvailable {
			get { return isAvailable; }
		}

		public Buddy() {
		}

		public Buddy(string screenName){
			this.screenName = screenName;
		}

		public Buddy(string screenName, string group){
			this.screenName = screenName;
			this.group = group;
		}
	}
}
