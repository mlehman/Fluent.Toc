using System;
using System.Text.RegularExpressions;

namespace Fluent.Toc {
	/// <summary>
	/// Summary description for TocUtility.
	/// </summary>
	public class TocUtility {
		private TocUtility() {
		}

		public static string StripHtml(string message){
			return Regex.Replace(message,@"<(.|\n)*?>", string.Empty);
		}
	}
}
