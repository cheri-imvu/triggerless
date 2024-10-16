<?php

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
$pid = $_GET['pid'];

$local_api_url = "http://localhost:61120/api/productsounds/${pid}"; #local-debug
$remote_api_url = "https://triggerless.com/api/productsounds/${pid}"; #production

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
