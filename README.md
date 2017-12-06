
# TldExtract

This .NET Standard 2 library and Command line tool extracts the root domain, subdomain name, 
and top level domain from a URL, using the [the Public Suffix List](http://www.publicsuffix.org).

This is based on the Go TldExtract and the Python TldExtract libraries from:
* https://github.com/joeguo/tldextract
* https://github.com/john-kurkowski/tldextract

This library is useful as it uses the public database to sort out what is the domain, the TLD
and the root domain, without using assumptions as to what the contents are.

Some examples:

| Host name                      | Subdomain    | Root domain     | Top-level Domain|
|--------------------------------|--------------|-----------------|-----------------|
| www.google.co.uk               | www          | google          | co.uk           |
| forums.news.cnn.com            | forums.news  | cnn             | com             |
| google.notavalidsuffix         | google       | notavalidsuffix |                 |
| media.forums.theregister.co.uk | media.forums | theregister     | co.uk           |
| www.cgs.act.edu.au             | www          | cgs             | act.edu.au      |
| joe.blogspot.co.uk             |              | joe             | blogspot.co.uk  |
| wiki.info                      |              | wiki            | info            |

# Usage

Add the TldExtract library NuGet package to your solution, and then create an instance of the 
`NStack.TldExtract` class.   You can either provide a path to the cache file where you want the 
public suffix list to be downloaded or nothing and the library will choose the proper cache
location for you.

Then invoke the Extract method that will return a tuple of values with the subdomain, the
root domain and the TLD domain.
 
# Examples

```

var extractor = new NStack.TldExtract ();
(var sub, var root, var tld) = extractor.Extract ("www.microsoft.com");
```
