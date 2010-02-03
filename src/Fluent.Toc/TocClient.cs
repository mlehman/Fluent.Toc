/*
 *  Fluent.Toc is a .NET component for communicating with 
 *  AOL's Instant Messenger (AIM) service. 
 * 
 *  Copyright 2004 by Fluent Consulting
 * 
 *  TocClient Class is a derivative work based on JavaTOC (http://www.jeffheaton.com/)
 *  Copyright 2002 by Jeff Heaton
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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Fluent.Text;
using System.Runtime.CompilerServices;

namespace Fluent.Toc {

	/// <summary>
	/// Represents the method that will handle the Message event of a TocClient class.
	/// </summary>
	public delegate void MessageEventHandler( object sender, MessageEventArgs e);
	
	/// <summary>
	/// Represents the method that will handle the Error event of a TocClient class.
	/// </summary>
	public delegate void ErrorEventHandler( object sender, ErrorEventArgs e);

	/// <summary>
	/// Represents the method that will handle the Flap event of a TocClient class.
	/// </summary>
	public delegate void FlapEventHandler( object sender, FlapEventArgs e);

	/// <summary>
	/// Represents the method that will handle the BuddyUpdate event of a TocClient class.
	/// </summary>
	public delegate void BuddyUpdateEventHandler( object sender, BuddyUpdateEventArgs e);

	/// <summary>
	/// Provides client connections for the TOC protocol. To process incoming message
	/// </summary>
	/// <example>
	/// The following example demonstrates how to send a message using TocClient.
	///	<code><![CDATA[
	/// TocClient tc = new TocClient();
	/// tc.SignIn("myscreenname","password");
	/// tc.Send("mybuddy","Hello!");
	/// tc.SignOut();
	/// ]]></code>
	/// </example>
	/// <example>
	/// The following example demonstrates how to receive messages using TocClient.
	///	<code><![CDATA[
	/// TocClient tc = new TocClient();
	/// tc.Message += new MessageEventHandler(OnMessage);
	/// tc.SignIn("myscreenname","password");
	/// tc.StartListening();
	/// ...
	/// protected void  OnMessage(object sender, MessageEventArgs e){
	/// WriteLine("{0}: {1}",e.From, e.Message);
	/// }
	/// ]]></code>
	/// </example>
	public class TocClient {

		public enum ProtocolVersion {
			TOCv1 = 1,
			TOCv2 = 2
		}

		private enum FrameType {
			SignOn = 1,
			Data = 2,
			Error = 3,
			SignOff = 4,
			KeepAlive = 5
		}

		private const string Language = "english";
		private const string Version = "TIC:Fluent.Toc";
		private const string RoastString = "Tic/Toc";
		private const int ThrottleDelay = 2000;

		private const string ClosedConnectionException = "An existing connection was forcibly closed by the remote host";
		private const string UnableToRead = "Unable to read data from the transport connection.";

		private NetworkStream stream;
		private BuddyInfoCollection buddies;
		private CapabilityCollection capabilities;
		private bool isShuttingDown;
		private Thread listeningThread;

		private ProtocolVersion protocol = ProtocolVersion.TOCv2;
		private string tocHost = "toc.oscar.aol.com";
		private int tocPort = 9898;
		private string authHost = "login.oscar.aol.com";
		private int authPort = 5190;

		private short sequence;
		private DateTime lastSent;
		private string screenName;


		/// <summary>
		/// Occurs when a message is received.
		/// </summary>
		public event MessageEventHandler Message;

		/// <summary>
		/// Occurs when a error message is received.
		/// </summary>
		public event ErrorEventHandler Error;

		/// <summary>
		/// Occurs when a flap is received. Allows monitoring of the lower level protocol.
		/// </summary>
		public event FlapEventHandler Flap;

		/// <summary>
		/// Occurs when configuration settings are received.
		/// </summary>
		public event EventHandler Config;

		/// <summary>
		/// Occurs on buddy arrival, departure, or updates.
		/// </summary>
		public event BuddyUpdateEventHandler BuddyUpdate;

		/// <summary>
		/// Occurs when disconnected.
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// The OSCAR authentication server
		/// </summary>
		public ProtocolVersion Protocol {
			get{ return protocol; }
			set{ protocol = value; }
		}

		/// <summary>
		/// The host address of the TOC server
		/// </summary>
		public string TocHost {
			get{ return tocHost; }
			set{ tocHost = value; }
		}

		/// <summary>
		/// The port used to connect to the TOC server
		/// </summary>
		public int TocPort {
			get{ return tocPort; }
			set{ tocPort = value; }
		}

		/// <summary>
		/// The OSCAR authentication server
		/// </summary>
		public string AuthHost {
			get{ return authHost; }
			set{ authHost = value; }
		}

		/// <summary>
		/// The OSCAR authentication server's port
		/// </summary>
		public int AuthPort {
			get{ return authPort; }
			set{ authPort = value; }
		}

		/// <summary>
		/// The screen name of the current user.
		/// </summary>
		public string ScreenName {
			get{ return screenName; }
		}

		/// <summary>
		/// Gets the collection of buddies added to the TocClient.
		/// </summary>
		/// <remarks>
		///  You can add BuddyInfos to the collection or remove BuddyInfos from the collection.
		///  The TocClient will notify the server to send buddy updates for the buddy accordingly.
		///  To pernamently store the list buddies contained BuddyInfoCollection to the server, use
		///  <see cref="SetConfig"/>.
		/// </remarks>
		public BuddyInfoCollection Buddies {
			get { return buddies; }
		}

		/// <summary>
		/// Gets the collection of Capabilities supported by the TocClient.
		/// </summary>
		/// <remarks>
		///  You can add Capabilites to the collection or remove Capabilites from the collection.
		///  The capabilities are sent to the server when first connected.  Modifying the collection
		///  after connected will not have a further effect.
		/// </remarks>
		public CapabilityCollection Capabilities {
			get { return capabilities; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the TocClient is connected to the server.
		/// </summary>
		public bool Connected {
			get { return stream != null; }
		}

		private int Code(string screenName, string password) {
			int sn = screenName[0] - 96;
			int pw = password[0] - 96;

			int a = sn * 7696 + 738816;
			int b = sn * 746512;
			int c = pw * a;
				
			return c - a + b + 71665152;
		}
			

		/// <summary>
		/// Creates a new instance of the TocClient class.
		/// </summary>
		public TocClient() {
			buddies = new BuddyInfoCollection(this);
			capabilities = new CapabilityCollection();
			lastSent = DateTime.Now;
		}

		/// <summary>
		/// Connects and signs on to the server.
		/// </summary>
		/// <param name="screenName">The screen name.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool SignOn(string  screenName, string password) {
			if (screenName == null) {
				throw new ArgumentNullException("screenName");
			}
			if (password == null) {
				throw new ArgumentNullException("password");
			}

			try {

				OpenConnection();
			
				SendRaw("FLAPON\r\n\r\n");
				ReadFlap();
				SendFlapSignOn(screenName);


				string flap = string.Format("toc2_signon {0} {1} {2} {3} {4} \"{5}\" 160 {6}",
					AuthHost,
					AuthPort,
					screenName,
					RoastPassword(password),
					Language,
					Version,
					Code(screenName, password)
					);
			
				SendFlap( FrameType.Data, flap);

				flap = ReadFlap();
		
				if ( IsErrorFlap(flap) ) {
					HandleError(flap.Substring(flap.IndexOf(':') + 1));
				}
			
				SendFlap(FrameType.Data,string.Format("toc_add_buddy {0}", screenName));
				SendFlap(FrameType.Data,"toc_init_done");
				SendCapabilities();
			

				this.screenName = screenName; 

				return true;
			} 
			catch (Exception e) {
				CloseConnection();
				throw e;
			}
		}


		/// <summary>
		/// Signs off and disconnects from the server.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SignOff() {
			StopListening();
			FlushStream();
			CloseConnection();
		}



		/// <summary>
		///  Send a message to a remote user.
		/// </summary>
		/// <param name="message">The instant message.</param>
		/// <param name="screenName">The remote user.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Send(string screenName, string message) {
			Send(screenName, message, false);
		}


		/// <summary>
		///  Send a message to a remote user.
		/// </summary>
		/// <param name="message">The instant message.</param>
		/// <param name="screenName">The remote user.</param>
		/// <param name="autoResponse">If true, then the auto response flag will be turned on for the message.</param>
		/// <remarks>Set <see cref="autoResponse"/> to true to if this is an automated response, such as a custom away message, rather than a response from the user.</remarks>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Send(string screenName, string message, bool autoResponse) {

			Throttle();
			
			string flap = null;
			if(protocol == ProtocolVersion.TOCv2){
				flap = string.Format("toc_send_im {0} \"{1}\"", Normalize(screenName), Encode(message));
			} else {
				flap = string.Format("toc_send_im2 {0} \"{1}\"", Normalize(screenName), Encode(message));
			}
			if(autoResponse) {
				flap += " auto";
			}

			SendFlap(FrameType.Data, flap);

			lastSent = DateTime.Now;
			
		}


		/// <summary>
		/// Starts processing events from the server.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StartListening() {
			if(listeningThread == null || !listeningThread.IsAlive) {
				ThreadStart threadStart = new ThreadStart(ProcessTocEvents);
				listeningThread = new Thread(threadStart);
				listeningThread.IsBackground = true;
				listeningThread.Start();
			}
		}

		/// <summary>
		/// Stops processing events from the server.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StopListening() {
			if(listeningThread != null && listeningThread.IsAlive) {
				isShuttingDown = true;
				listeningThread.Abort();
				listeningThread.Join();
			}
		}


		/// <summary>
		/// Evil/Warn someone else. You can only evil people who have recently sent you
		/// ims.  The higher someones evil level, the slower they can send message.
		/// </summary>
		/// <param name="screenName">Screen name of person to warn.</param>
		/// <param name="anonymous">Whether this is an anonymous warning.</param>
		public void Warn(string screenName, bool anonymous) {
			if(anonymous) {
				SendFlap(FrameType.Data,"toc_evil " + screenName + " anon");
			} 
			else {
				SendFlap(FrameType.Data,"toc_evil " + screenName + " norm");
			}
		}


		/// <summary>
		/// Set the status to away.
		/// </summary>
		/// <param name="awayMessage">The away message.</param>
		public void SetAway(string awayMessage) {
			SendFlap(FrameType.Data, "toc_set_away " + awayMessage);
		}


		/// <summary>
		/// Set the status to back.
		/// </summary>
		public void SetBack() {
			SendFlap(FrameType.Data, "toc_set_away");
		}


		/// <summary>
		/// Set idle information. If idleSeconds is 0 then the user isn't idle at all.
		/// If idleSeconds is greater then 0 then the user has already been idle
		///	for idleSeconds number of seconds.  The server will automatically
		/// keep incrementing this number, so do not repeatedly call with new
		/// idle times.
		/// </summary>
		/// <param name="idleSeconds">The number of seconds the user has been idle.</param>
		public void SetIdle(int idleSeconds) {
			SendFlap(FrameType.Data,  "toc_set_idle " + idleSeconds);
		}


		/// <summary>
		/// Sets the server configuration to match the contents of <see cref="Buddies"/>.
		/// </summary>
		public void SetConfig() {
			BuddyInfo[] buddies = Buddies.SortBuddyList();

			StringBuilder config = new StringBuilder();
			string group = null;
			foreach(BuddyInfo buddy in buddies) {
				if(group != buddy.Group) {
					group = buddy.Group;
					config.Append("g " + group + "\n");
				}
				config.Append("b " + buddy.ScreenName + "\n");
			}

			SendFlap(FrameType.Data,"toc_set_config \"" + config.ToString() + "\"");
		}

		/// <summary>
		/// ADD the following people to your permit mode.  If
		/// you are in deny mode it will switch you to permit
		/// mode first.  With no arguments and in deny mode
		/// this will switch you to permit none. If already
		/// in permit mode, no arguments does nothing
		/// and your permit list remains the same.
		/// </summary>
		public void Permit(params string[] screenNames) {
			SendFlap(FrameType.Data,"toc_add_permit" + BuildList(screenNames) );
		}
		
		/// <summary>
		/// ADD the following people to your deny mode. If
		/// you are in permit mode it will switch you to
		/// deny mode first.  With no arguments and in permit
		/// mode, this will switch you to deny none. If
		/// already in deny mode, no arguments does nothing
		/// and your deny list remains unchanged.
		/// </summary>
		public void Deny(params string[] screenNames) {
			SendFlap(FrameType.Data,"toc_add_deny" + BuildList(screenNames));
		}


		/// <summary>
		///	Change a user's password.  An ADMIN_PASSWD_STATUS or ERROR message will 
		///	be sent back to the client.
		/// </summary>
		/// <param name="oldPassword">The user's old password.</param>
		/// <param name="newPassword">The new password.</param>
		public void ChangePassword(string oldPassword, string newPassword) {
			SendFlap(FrameType.Data,"toc_change_passwd " + oldPassword + " " + newPassword);
		}


		/// <summary>
		/// Elapsed Time since last message sent
		/// </summary>
		private int RemainingThrottleDelay {
			get { return ThrottleDelay - (int)DateTime.Now.Subtract(lastSent).TotalMilliseconds; }
		}


		private void OpenConnection() {

			IPHostEntry hostEntry = Dns.Resolve(this.TocHost);

			for( int i = 0; i < hostEntry.AddressList.Length ; i++) {
				
				try {

					IPAddress address = hostEntry.AddressList[i];
					IPEndPoint ipe = new IPEndPoint(address, this.TocPort);
					Socket socket = new Socket(
						ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect(ipe);

					if(socket.Connected) {
						stream = new NetworkStream(socket, true);
						break;
					} 
					else {
						continue;
					}

				} 
				catch (SocketException se) { 
				
					if(i == hostEntry.AddressList.Length - 1) {
						throw se;
					}

				}
			}


		}


		private void FlushStream() {
			if(stream != null) {
				stream.Flush();
				Throttle();
			}
		}


		private void CloseConnection() {
			if(stream != null) {
				stream.Close();
				screenName = null;
				stream = null;
			}
		}


		private void SendRaw(string flap) {
			byte[] buffer = GetBytes(flap);
			stream.Write(buffer,0,buffer.Length);
		}


		private void SendFlap(FrameType type, string flap) {

			//TODO: check connections

			int length = flap.Length+1;
			sequence++;
			stream.WriteByte((byte)'*');
			stream.WriteByte((byte)type);
			WriteWord(sequence);
			WriteWord((short)length);
			byte[] buffer = GetBytes(flap);
			stream.Write(buffer, 0, buffer.Length);
			stream.WriteByte(0);
			stream.Flush();

			if(Flap != null) {
				Flap(this, new FlapEventArgs(flap));
			}
		}


		private void SendFlapSignOn(string screenName) {

			int length = 8 + screenName.Length;
			sequence++;

			stream.WriteByte((byte)'*');
			stream.WriteByte((byte)FrameType.SignOn);

			WriteWord(sequence);
			WriteWord((short)length);
			
			stream.WriteByte((byte)0);
			stream.WriteByte((byte)0);
			stream.WriteByte((byte)0);
			stream.WriteByte((byte)1);

			stream.WriteByte((byte)0);
			stream.WriteByte((byte)1);

			WriteWord((short)screenName.Length);
			byte[] buffer = GetBytes(screenName);
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}


		private string ReadFlap() {
			string flap = null;
			try {

				if(stream == null) {
					throw new ClosedConnectionException("NetworkStream is uninitalized.");
				}

				if ( stream.ReadByte()!='*' ) {
					return null;
				}

				stream.ReadByte();
				stream.ReadByte();
				stream.ReadByte();

				int length = (stream.ReadByte()*0x100)+stream.ReadByte();
				byte[] buffer = new byte[length];
				stream.Read(buffer, 0, length);

				System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();


				flap = encoding.GetString(buffer, 0, length);
			} 
			catch (IOException ioe) {
				if(ioe.Message == ClosedConnectionException || ioe.Message == UnableToRead) {
					
					if (Disconnected != null && !isShuttingDown) {
						Disconnected(this, EventArgs.Empty);
					}
				}

				throw ioe;
			}

			if(Flap != null && flap != null) {
				Flap(this, new FlapEventArgs(flap));
			}

			return flap;
		}


		private byte[] GetBytes(string s) {
			System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			return encoding.GetBytes(s);
		}


		private void WriteWord(short word) {
			stream.WriteByte((byte)((word >> 8) & 0xff));
			stream.WriteByte((byte)(word & 0xff));
		}


		private string RoastPassword(string password) {
			byte[] xor = GetBytes(RoastString);
			int xorIndex = 0;
			String rtn = "0x";

			for ( int i = 0; i < password.Length; i++ ) {
				string hex = string.Format("{0:X}",(xor[xorIndex]^(int)password[i]));
				if ( hex.Length == 1 ) {
					hex = "0" + hex;
				}
				rtn+=hex;
				xorIndex++;
				if ( xorIndex==xor.Length ) {
					xorIndex=0;
				}
			}
			return rtn;
		}


		#region Processing TOC Events

		private void ProcessTocEvents() {
			isShuttingDown = false;
			while (!isShuttingDown) {
				string flap = ReadFlap();
				if ( flap == null ) {
					continue;
				}

				StringTokenizer stk = new StringTokenizer(flap,':');

				
				switch(stk.ReadToken().ToUpper()) {
					case "IM_IN":
						HandleIM(stk.ReadToEnd());
						break;
					case "IM_IN2":
						HandleIM(stk.ReadToEnd());
						break;
					case  "ERROR":
						HandleError(stk.ReadToEnd());
						break;
					case "CONFIG":
						HandleConfig(stk.ReadToEnd());
						break;
					case "CONFIG2":
						HandleConfig(stk.ReadToEnd());
						break;
					case "UPDATE_BUDDY":
						HandleUpdate(stk.ReadToEnd());
						break;
					case "UPDATE_BUDDY2":
						HandleUpdate(stk.ReadToEnd());
						break;
				}
			} 
		}


		private bool IsErrorFlap(string flap) {
			return flap.ToUpper().StartsWith("ERROR");
		}


		private void HandleIM(String flap) {

			if(Message != null) {
				StringTokenizer stk = new StringTokenizer(flap,':');

				string from = stk.ReadToken();
				bool autoResponse = stk.ReadToken() == "T";

				if(protocol == ProtocolVersion.TOCv2){
					string unknownParam = stk.ReadToken();
				}

				string message = stk.ReadToEnd();
				
				MessageEventArgs e = new MessageEventArgs(from, message, autoResponse);
				Message(this,e);
			}
		}


		private void HandleUpdate(String flap) {

			StringTokenizer stk = new StringTokenizer(flap,':');

			string screenName = stk.ReadToken();

			BuddyInfo buddy = Buddies[screenName];

			if(buddy != null) {

				buddy.screenName = screenName;

				buddy.online = stk.ReadToken() == "T";
				buddy.evilAmount = int.Parse(stk.ReadToken());

				long epochDate = long.Parse(stk.ReadToken());
				buddy.signOnTime = new DateTime(1970,1,1).AddSeconds(epochDate);

				int idleMinutes = int.Parse(stk.ReadToken());
				buddy.idleTime = new TimeSpan(0,idleMinutes,0);

				buddy.isOnAol = stk.ReadChar() == 'A';

				switch(stk.ReadChar()) {
					case 'A':
						buddy.userClass = BuddyUserClass.Admin;
						break;
					case 'U':
						buddy.userClass = BuddyUserClass.Unconfirmed;
						break;
					default:
						buddy.userClass = BuddyUserClass.Normal;
						break;
				}

				if(stk.HasMoreTokens) {
					buddy.isAvailable = stk.ReadChar() != 'U';
				} 
				else {
					buddy.isAvailable = true;
				}

				if(BuddyUpdate != null) {
					BuddyUpdate(this, new BuddyUpdateEventArgs(buddy));
				}
			}
			
		}


		private void HandleConfig(String flap) {

			StringReader sr = new StringReader(flap);

			string group = string.Empty, buddyName = string.Empty;

			while(sr.Peek() != -1) {
				switch(sr.Read()) {
					case 'g':
						if(Protocol == ProtocolVersion.TOCv2) {
							sr.Read();
						}
						group = sr.ReadLine().Trim();
						break;
					case 'b':
						if(Protocol == ProtocolVersion.TOCv2) {
							sr.Read();
						}
						buddyName = sr.ReadLine().Trim();
						Buddies.Add(new BuddyInfo(buddyName, group));
						break;
				}
			}

			if(Config != null) {
				Config(this, EventArgs.Empty);
			}
			
		}


		private void HandleError(String flap) {

			string e = string.Empty;
			string v = string.Empty;

			StringTokenizer stk = new StringTokenizer(flap,':');

			if(stk.HasMoreTokens) {
				e = stk.ReadToken(); 
			}
			if(stk.HasMoreTokens) {
				v = stk.ReadToken();
			}

			string message = null;
			switch(e) {
				case "901":
					message = string.Format("{0} is not currently available.", v);
					break;
				case "902":
					message = string.Format("Warning of {0} is not currently available.", v);
					break;
				case "903":
					message = string.Format("A message has been dropped, you are exceeding the server speed limit", v);
					break;
				case "980":
					throw new TocException("Incorrect nickname or password.");
				case "981":
					throw new TocException("The service is temporarily unavailable.");
				case "982":
					throw new TocException("Your warning level is currently too high to sign on.");
				case "983":
					throw new TocException("You have been connecting and disconnecting too frequently.  Wait 10 minutes and try again. If you continue to try, you will need to wait even longer.");
				case "989":
					throw new TocException("An unknown signon error has occurred " + v);
			}

			if(message != null) {
				if(Error != null) {
					Error( this, new ErrorEventArgs(int.Parse(e), message, v));
				}
			} 
			else {
				throw new TocException(flap);
			}
		}


		#endregion

		private string Normalize(String screenName) {
			string normalized = "";
			for ( int i=0; i < screenName.Length; i++ ) {
				if ( screenName[i] == ' ' ) {
					continue;
				}
				normalized += Char.ToLower(screenName[i]);
			}
			return normalized;
		}


		private string Encode(string message) {
			System.Text.StringBuilder encoded = new System.Text.StringBuilder();
			for ( int i = 0; i < message.Length; i++ ) {
				switch ( message[i] ) {
					case '\r':
						encoded.Append("<br>");
						encoded.Append(message[i]);
						break;
					case '{':
						encoded.Append("\\");
						encoded.Append(message[i]);
						break;
					case '}':
						encoded.Append("\\");
						encoded.Append(message[i]);
						break;
					case '\\':
						encoded.Append("\\");
						encoded.Append(message[i]);
						break;
					case '"':
						encoded.Append("\\");
						encoded.Append(message[i]);
						break;
					default:
						encoded.Append(message[i]);
						break;
				}
			}
			return encoded.ToString();

		}


		private void AddBuddies() {
			
			string buddyList = string.Empty;
			foreach(BuddyInfo b in Buddies) {
				buddyList += " " + b.ScreenName;
			}
			SendFlap(FrameType.Data,"toc_add_buddy" + buddyList);
		}


		internal void AddBuddy(string screenName) {
			if(protocol == ProtocolVersion.TOCv2){ 
				BuddyInfo b = Buddies[screenName];
				if(b == null){
					throw new TocException("Buddy not in BuddyCollection.");
				}
				SendFlap(FrameType.Data,"toc2_new_buddies {g:" + b.Group + "\nb:" + screenName + "}");
			} else {
				SendFlap(FrameType.Data,"toc_add_buddy " + screenName);
			}
		}


		internal void RemoveBuddy(string screenName) {
			if(protocol == ProtocolVersion.TOCv2){
				BuddyInfo b = Buddies[screenName];
				if(b == null){
					throw new TocException("Buddy not in BuddyCollection.");
				}
				SendFlap(FrameType.Data,"toc2_remove_buddy " + screenName + " " + b.Group);
			} else {
				SendFlap(FrameType.Data,"toc_remove_buddy " + screenName);
			}
		}


		private void SendCapabilities() {
			if(Capabilities.Count > 0) {
				string flap = "toc_set_caps";
				foreach(Capability capability in Capabilities) {
					flap += " " + capability.UUID;
				}
				SendFlap(FrameType.Data, flap);
			}
		}


		private string BuildList(params string[] items) {
			StringBuilder list = new StringBuilder();
			foreach(string item in buddies) {
				list.Append(" " + item);
			}
			return list.ToString();
		}


		private void Throttle() {
			int remainingMilliseconds = RemainingThrottleDelay;
			if(remainingMilliseconds > 0) {
				Thread.Sleep(remainingMilliseconds);
			}
		}


	}
}
