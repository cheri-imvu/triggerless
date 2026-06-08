<!DOCTYPE html>
<html>
  <head>
    <title>Triggerless - Expose IMVU Outfits</title>
    <meta charset="UTF-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="css/outfits.css" />
    <link rel="icon" href="/favicon.ico" />
    <meta name="theme-color" content="#001D32" />
    <meta name="description" content="Expose IMVU Outfits" />
    <meta property="og:title" content="Triggerless - Expose IMVU Outfits" />
    <meta property="og:description" content="View and analyze hidden IMVU outfits, products, and creators quickly and accurately." />
    <meta property="og:image" content="https://www.triggerless.com/i/logo-u.png" />
    <meta property="og:url" content="https://www.triggerless.com/" />
    <meta property="og:type" content="website" />
    <meta name="twitter:card" content="summary_large_image" />
    <meta name="twitter:title" content="Triggerless - Expose IMVU Outfits" />
    <meta name="twitter:description" content="View and analyze IMVU outfits, products, and creators quickly and accurately." />
    <meta name="twitter:image" content="https://www.triggerless.com/i/logo-u.png" />
    <link rel="canonical" href="https://www.triggerless.com/" />
  </head>
<body>
  <header>
    <div class="banner-image"><a href="/"><img src="https://www.triggerless.com/i/logo-u.png"/></a></div>
    <h1>Expose IMVU Outfits</h1>
  </header>
  <main>
    <div class="input-container">
      <div class="input-blab">
        <ul>
          <li>In IMVU, right-click the scene, click "View Products in this scene"</li>
          <li>Select the whole link and copy it to Clipboard (Ctrl-C)</li>
          <li>Paste the IMVU-generated link into the text box to the right.</li>
          <li>Wait a few seconds for the avatars to load.</li>
          <li>Expand any outfit to see what's there.</li>
          <li>This is free for now, but at some point you'll have to get a subscription.</li>
        </ul>
      </div>
      <div class="input-entry">
        <textarea id="input"></textarea> 
      </div>
      <div class="input-controls">
        <button id="paste">Paste URL</button>
        <button id="clear">Clear</button>
      </div>
    </div>
    <div id="loading">
      Loading Avatars, please wait...
    </div>
    <div id="outfits-container">
    </div>
  </main>
  <script src="outfits.js" defer="defer"></script>
</body>
</html>
