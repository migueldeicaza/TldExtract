//
// Based on the Python and Go code for Tld Extraction from:
// https://github.com/john-kurkowski/tldextract
// https://github.com/joeguo/tldextract
//
// Miguel de Icaza
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Diagnostics;

namespace NStack
{
	/// <summary>
	/// Top Level Domain Extractor
	/// </summary>
	/// <remarks>
	/// This computes the subdomain, domain and toplevel domain from a hostname, 
	/// based on the publicly maintained public suffic list from https://publicsuffix.org/
	/// </remarks>
	public class TldExtract
	{
		Trie rootNode;

		class Trie
		{
			public bool ExceptRule, ValidTld;
			public Dictionary<string, Trie> matches = new Dictionary<string, Trie> ();
		}

		string Download (string file)
		{
			var client = new HttpClient ();
			var tlds = client.GetStringAsync ("https://publicsuffix.org/list/public_suffix_list.dat").Result;

			var tmp = file + Process.GetCurrentProcess ().Id;
			using (var output = File.CreateText (tmp))
				output.Write (tlds);

			try {
				File.Move (tmp, file);
			} catch { }
			return tlds;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NStack.TldExtract"/> class, if a cache file is passed, it is used, otherwise a default is computed
		/// </summary>
		/// <param name="cacheFile">Cache file.</param>
		/// <remarks>
		/// This constructor can raise an exception if there is an IO Error.
		/// </remarks>
		public TldExtract (string cacheFile = null)
		{
			if (cacheFile == null) {
				var cacheDir = Environment.GetFolderPath (Environment.SpecialFolder.InternetCache);
				if (!Directory.Exists (cacheDir)) {
					try {
						Directory.CreateDirectory (cacheDir);
					} catch {
						cacheFile = Path.GetTempFileName ();
					}
				} 
				if (cacheFile == null)
					cacheFile = Path.Combine (cacheDir, "public_suffix_list.dat");
			}

			var lines = File.Exists (cacheFile) ? File.ReadAllLines (cacheFile) : Download (cacheFile).Split ('\n');

			rootNode = new Trie () {
				ExceptRule = false,
				ValidTld = false
			};
			foreach (var line in File.ReadAllLines (cacheFile)){
				var l = line.Trim ();
				if (l== "" || l.StartsWith ("//", StringComparison.Ordinal))
					continue;
				bool exceptionRule = l[0] == '!';
				if (exceptionRule)
					l = l.Substring (1);
				AddTldRule (rootNode, l.Split ('.'), exceptionRule);
			}
		}

		void AddTldRule (Trie root, string [] labels, bool exception)
		{
			var t = root;
			for (int i = labels.Length - 1; i >= 0; i--){
				var label = labels [i];
				Trie m;

				if (!t.matches.TryGetValue (label, out m)){
					m = new Trie () {
						ExceptRule = exception,
						ValidTld = !exception && i == 0
					};

					t.matches [label] = m;
				}
				else if (i == 0) {
					m.ValidTld = true;
				}
				t = m;
			}
		}

		(string subDomain, string rootDomain) Subdomain (string host)
		{
			var elements = host.Split ('.');
			var l = elements.Length;
			if (l == 1)
				return ("", host);
			return (string.Join (".", elements, 0, l - 1), elements [l - 1]);
		}

		(int tldIndex, bool valid) GetTldIndex (string [] labels)
		{
			var t = rootNode;
			var parentValid = false;
			for (int i = labels.Length - 1; i >= 0; i--) {
				var lab = labels [i];

				var found = t.matches.TryGetValue (lab, out var n);
				var starFound = t.matches.ContainsKey ("*");

				if (found && !n.ExceptRule) {
					parentValid = n.ValidTld;
					t = n;
				} else if (parentValid)
					return (i + 1, true);
				else if (starFound)
					parentValid = true;
				else
					return (-1, false);
			}
			return (-1, false);
		}

		/// <summary>
		/// Extract the Subdomain, root domain and top level domain from the specified hostname.
		/// </summary>
		/// <returns>Strings for the subdomain, root domaind and top-level domain.</returns>
		/// <param name="host">A DNS Host, you can get this from a Uri object by accessing the Host property, when HostNameType is of type Dns.</param>
		public (string sub, string root, string tld) Extract (string host)
		{
			var elements = host.Split ('.');
			(var tldIndex, var validTld) = GetTldIndex (elements);
			string domain, tld;

			if (validTld) {
				domain = string.Join (".", elements, 0, tldIndex);
				tld = string.Join (".", elements, tldIndex, elements.Length - tldIndex);
			} else {
				tld = "";
				domain = host;
			}
			(var sub, var root) = Subdomain (domain);
			return (sub, root, tld);
		}
	}
}
