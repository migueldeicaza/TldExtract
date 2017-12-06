using NUnit.Framework;
using System;
using NStack;

namespace TldExtractTestDesktop
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void TestCase ()
		{
			var x = new TldExtract ();

			var tests = new [] {
				("www.google.com", "www", "google", "com"),
				("www.google.co.uk", "www", "google", "co.uk"),
				("joe.blogspot.co.uk", "", "joe", "blogspot.co.uk"),
				("www.github.com", "www", "github", "com"),
				("media.forums.theregister.co.uk", "media.forums", "theregister", "co.uk"),
				("216.22.project.coop", "216.22", "project", "coop"),
				("wiki.info", "", "wiki", "info"),
				("www.cgs.act.edu.au", "www", "cgs", "act.edu.au"),
				("www.metp.net.cn", "www", "metp", "net.cn"),
			};
			foreach ((var host, var sub, var root, var tld) in tests) {
				(var nsub, var nroot, var ntld) = x.Extract (host);
				var msg = "On extracting " + host;
				Assert.AreEqual (sub, nsub, msg);
				Assert.AreEqual (root, nroot, msg);
				Assert.AreEqual (tld, ntld, msg);
			}
		}
	}
}
