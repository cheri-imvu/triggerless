<?php

/*
    This goes onto the CORS server, not triggerless.com
*/

header('Content-Type: application/json');
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

if (!isset($_GET['url'])) {
    http_response_code(400);
    echo json_encode(['error' => 'Missing url parameter']);
    exit;
}

$url = $_GET['url'];
$cookieValue = "01234567890123456789012345678901";

$ch = curl_init($url);

curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
curl_setopt($ch, CURLOPT_TIMEOUT, 10);
curl_setopt($ch, CURLOPT_COOKIE, "osCsid=$cookieValue");

$response = curl_exec($ch);

if ($response === false) {
    http_response_code(500);
    echo json_encode([
        'error' => 'cURL error',
        'message' => curl_error($ch)
    ]);
    curl_close($ch);
    exit;
}

$httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

curl_close($ch);

// If upstream server returned an error
if ($httpCode !== 200) {
    http_response_code($httpCode);
    echo json_encode([
        'error' => 'Upstream request failed',
        'status' => $httpCode,
        'body' => $response
    ]);
    exit;
}

// Success
echo $response;
?>