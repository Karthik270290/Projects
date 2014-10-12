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

### Current limitations

- The @route directive has to be on its own line, and has to be commented out (RazorStyle or C# style).
- Routed pages are only picked up by precompilation

### What's to do to finish the prototype

##### Higher Pri
- Auto compute web pages at startup 
-- Scan + Add just in precompilation, do it on a background thread. And block.
- Make route a real attribute and a real directive
- Add a file watcher to scan for changes in the routed pages folder, and update the action descriptors collection when it happens

##### Lower Pri
- pick up route from runtime compilation
- Throw on route at runtime, when route is not inside the right path.
- Support multiple folders, mapped to multiple url patterns.