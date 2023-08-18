<?php

header('Content-Type: application/json');
$queryString = $_SERVER['QUERY_STRING'];

$remote_api_url = "https://triggerless.com/api/products?${queryString}";

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
