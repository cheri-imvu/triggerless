<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Live Stream Player</title>
  <style>
    body {
      font-family: sans-serif;
      background: #203;
      color: powderblue;
      text-align: center;
      padding: 2em;
    }
    #nowPlaying {
      font-size: 1.0em;
      margin-top: 0.6em;
	  color: powderblue;
    }
    audio {
      width: 80%;
      max-width: 400px;
      margin-top: 2em;
	  color: powderblue;
    }
	h1 {
	  margin: 0px;
	  font-size: 1.3em;
	  margin-block-start: 0.25em;
	  margin-block-end: 0.25em;
	}
  </style>
</head>
<body>

  <h1>🎧 Triggerless Radio</h1>
  <audio controls autoplay>
    <source src="http://live.triggerless.com:6969/live.mp3" type="audio/mpeg">
    Your browser does not support the audio element.
  </audio>

  <div id="nowPlaying">Loading current track...</div>

  <script>
    async function fetchNowPlaying() {
      try {
        const response = await fetch('http://live.triggerless.com:6969/status-json.xsl');
        if (!response.ok) throw new Error("Failed to fetch Icecast status");

        const data = await response.json();
        const source = data.icestats.source;

        // Handle single or multiple mountpoints
        const title = Array.isArray(source)
          ? source.find(s => s.listenurl.includes('/live.mp3'))?.title
          : source.title;

        document.getElementById("nowPlaying").textContent =
          title ? `Now Playing: ${title}` : 'Now Playing: (unknown)';
      } catch (err) {
        document.getElementById("nowPlaying").textContent = "Unable to fetch track info";
        console.error(err);
      }
    }

    fetchNowPlaying();
    setInterval(fetchNowPlaying, 10000); // Update every 10 seconds
  </script>

</body>
</html>
