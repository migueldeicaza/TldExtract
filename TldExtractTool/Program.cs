// 
// Command line front-end to the TldExtract library
//

using System;
using Mono.Options;
using System.IO;
using NStack;

class TldExtractTool
{
	static void UpdateCache ()
	{
	}

	static int Main (string [] args)
	{
		string cache = null;
		bool showHelp = false;
		bool updateCache = false;

		var options = new OptionSet {
			{ "update", "Update the cache", n => updateCache = true },
			{ "c|cache=", "Specifies a cache file to use", v => cache = v },
			{ "h|help", "show this message and exit", h => showHelp = true },
		};
		try {
			var extra = options.Parse (args);

			if (showHelp) {
				options.WriteOptionDescriptions (Console.Out);
				return 0;
			}
			var x = new TldExtract (cache);

			if (updateCache) 
				return 0;
			
			foreach (var arg in extra) {
				string host;
				Uri u;
				if (Uri.TryCreate (arg, UriKind.RelativeOrAbsolute, out u))
					host = u.Host;
				else
					host = arg;

				(var sub, var root, var tld) = x.Extract (host);
				Console.WriteLine ($"{arg} subdomain={sub} root={root} tld={tld}");
			}

		} catch (OptionException) {
			Console.WriteLine ("Use tldextract --help for details");
			return 1;
		}
		return 0;
	}
}
