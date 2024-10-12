<?php
// Set the appropriate headers to indicate that the response contains JSON data
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
$id = $_GET["id"];

// Replace 'https://example.com/api/endpoint' with the actual URL of the remote server's API endpoint
$local_api_url = "https://localhost:61120/api/user/${id}"; #local-debug
$remote_api_url = "https://triggerless.com/api/user/${id}"; #production


// Make the HTTPS request to the remote server and get the JSON data
$remote_json_data = file_get_contents($remote_api_url);

// If the request was successful and data was retrieved
if ($remote_json_data !== false) {
    echo $remote_json_data;
} else {
    // If there was an error fetching the data from the remote server, return an error JSON response
    $error_response = json_encode(array('error' => 'Unable to fetch data from the remote server.'));
    echo $error_response;
}
?>
