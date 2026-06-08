<?php

/*
    This goes onto the CORS server, not triggerless.com
*/

header('Content-Type: application/json');
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

if (!isset($_GET['pid'])) {
    http_response_code(400);
    echo json_encode(['error' => 'Missing pid parameter']);
    exit;
}

$pid = $_GET['pid'];
$url = "https://www.imvu.com/shop/derivation_tree.php?products_id=$pid";
$cookieValue = "514181e69775fb909936e5824c92ca9d";

$ch = curl_init($url);

curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
curl_setopt($ch, CURLOPT_TIMEOUT, 10);
curl_setopt($ch, CURLOPT_COOKIE, "osCsid=$cookieValue");
curl_setopt($ch, CURLOPT_ENCODING, "");
curl_setopt($ch, CURLOPT_USERAGENT, "Mozilla/5.0");

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

// Try simple title extraction first (more reliable than DOM here)
if (preg_match('/<title>(.*?)<\/title>/is', $response, $match)) {
    $title = html_entity_decode($match[1], ENT_QUOTES | ENT_HTML5);

    if (preg_match('/IMVU Product:\s*(.*?)\s*:\s*Derivation Tree/i', $title, $matches)) {
        echo json_encode([
            'pid' => $pid,
            'name' => $matches[1]
        ]);
        exit;
    }

    // Title found but format unexpected
    echo json_encode([
        'error' => 'Title format unexpected',
        'title' => $title
    ]);
    exit;
}

// If we get here, title wasn't found at all
echo json_encode([
    'error' => 'No <title> tag found',
    'sample' => substr($response, 0, 500)
]);
?>