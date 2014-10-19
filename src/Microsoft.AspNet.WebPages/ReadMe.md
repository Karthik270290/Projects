## The webpages framework package - prototype

### What's the status
This is pretty much just a working prototype

It supports serving web pages from anywhere in the app
It supports specifying routes inside pages, if they are in a RoutePages folder

### How to use

in the services section in startup.cs add the following line:
	services.AddWebPages();

you can configure the WebPagesOptions, to allow for various scanning folders, and base url paths.

Add pages to the folders you specified, by default normal pages serve from anywhere, routed pages from \RoutedPages.

Add the @route directive like the following
@route /foo/{bar}

similarly httpget, httppost, httpdelete, httpput and httppatch create routes constrained to the matching verb.

### What's to do to finish the prototype -  10/18/2014

##### Higher Pri
 
- Parameter binding - @bind directive? Q: Should bind be a general feature and not related to web pages?
- Filters - @attribute directive -> @attribute [Authorize] and the authorize is placed on the controller? Should attributes be a general feature? We can for now support attributes with something simple like :

@functions {
	[PlaceYourAttributeHere]
	__WellKnownMethodName() { }
}

Or with one small step :
@attributes {
	[PlaceYourAttributesHere]
	// codegen __WellKnownMethodName() { }
}

- Code blocks around httpget

@httpget /foo/bar
{
	// code that executes only for this route
}

##### Lower Pri
- Throw on route at runtime, when route is not inside the right path.
- Support multiple folders, mapped to multiple url patterns.
- Support predicate or rejection pattern, so webpages can work with MVC but still serve pages from root (so no collission in the views folder).
- Add a file watcher to scan for changes in the routed pages folder, and update the action descriptors collection when it happens.
-- [perf] Scan on a background thread right at startup, and block the first request.
