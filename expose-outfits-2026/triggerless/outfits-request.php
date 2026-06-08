<?php

// --- Allowed origins ---
$allowedOrigins = [
    "https://www.triggerless.com",
    "https://triggerless.com",
    "http://localhost:8080"
];

// Get request origin
$origin = $_SERVER['HTTP_ORIGIN'] ?? "";

// Set CORS header only if allowed
if (in_array($origin, $allowedOrigins)) {
    header("Access-Control-Allow-Origin: $origin");
}

// Always include these
header("Access-Control-Allow-Methods: POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

// Handle preflight
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

// Only allow POST
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Only POST allowed']);
    exit;
}

// Get raw POST body
$input = file_get_contents("php://input");

// Forward request
$ch = curl_init("https://www.triggerless.com/api/outfits/request");

curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_POST, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, $input);
curl_setopt($ch, CURLOPT_HTTPHEADER, [
    "Content-Type: application/json",
    "Content-Length: " . strlen($input)
]);
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

// Forward upstream status
$httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
http_response_code($httpCode);

curl_close($ch);

// Return response
header("Content-Type: application/json");
echo $response;