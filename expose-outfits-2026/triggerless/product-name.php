<?php

header('Content-Type: application/json');

if (isset($_GET['pid']))
{
	$url = "http://192.3.55.225/product-name.php?pid=" . $_GET['pid'];
}
else
{
    http_response_code(400);
    echo json_encode(['error' => 'Missing pid parameter']);
    exit;
}

$ch = curl_init($url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
curl_setopt($ch, CURLOPT_TIMEOUT, 10);
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