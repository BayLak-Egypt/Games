<?php
// Configuration
$serverIp = '192.168.1.6';  // Replace with your server IP
$serverPort = 12203;        // Replace with your server port
$rconPassword = 'test123';  // Replace with your RCON password
$dataFile = 'player_data.txt';

// Function to send RCON command and get response
function sendRconCommand($command, $ip, $port, $password) {
    $fp = fsockopen("udp://$ip", $port, $errno, $errstr, 1);

    if (!$fp) {
        return "Network error: $errstr ($errno)";
    }

    $message = "\xFF\xFF\xFF\xFF\x02rcon $password $command";
    fwrite($fp, $message);

    $response = fread($fp, 4096);
    fclose($fp);

    return $response;
}

// Function to load player data from file
function loadPlayerData($filePath) {
    $data = [];
    if (file_exists($filePath)) {
        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
        $currentMap = '';

        foreach ($lines as $line) {
            if (strpos($line, 'map:') === 0) {
                $currentMap = trim(substr($line, 4));
            } elseif (strpos($line, 'name:') === 0) {
                list(, $name, , $score) = explode(' ', $line);
                // Only include names that contain letters
                if (preg_match('/[a-zA-Z]/', $name)) {
                    $data[$name] = ['score' => (int)$score];
                }
            }
        }
    }
    return ['map' => $currentMap, 'players' => $data];
}

// Function to save player data to file
function savePlayerData($filePath, $mapName, $playerData) {
    $file = fopen($filePath, 'c+');
    if ($file) {
        flock($file, LOCK_EX); // Lock file for writing
        ftruncate($file, 0);   // Clear the file content
        rewind($file);
        fwrite($file, "map: $mapName\n\n");
        foreach ($playerData as $name => $data) {
            fwrite($file, "name: $name score: {$data['score']}\n");
        }
        fflush($file);          // Flush output to the file
        flock($file, LOCK_UN); // Unlock file
        fclose($file);
    }
}

// Fetch the current status
$response = sendRconCommand('status', $serverIp, $serverPort, $rconPassword);

// If there's an error with RCON password, display an error message
if (strpos($response, 'Bad rconpassword') !== false) {
    echo "Invalid RCON password: $rconPassword";
    exit;
}

// Process the response and update player data
$playerData = loadPlayerData($dataFile);
$lines = explode("\n", $response);
$isDataSection = false;
$mapName = '';

foreach ($lines as $line) {
    if (strpos($line, 'map:') === 0) {
        $mapName = trim(substr($line, 4));
    } elseif (strpos($line, 'num score ping name') !== false) {
        $isDataSection = true;
    } elseif ($isDataSection) {
        $parts = preg_split('/\s+/', trim($line));
        if (count($parts) >= 4 && is_numeric($parts[1])) {
            $name = $parts[3];
            $score = (int)$parts[1];

            // Only include names that contain letters
            if (preg_match('/[a-zA-Z]/', $name)) {
                if (isset($playerData['players'][$name])) {
                    $playerData['players'][$name]['score'] = $score;
                } else {
                    $playerData['players'][$name] = ['score' => $score];
                }
            }
        }
    }
}

savePlayerData($dataFile, $mapName, $playerData['players']);
?>


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Server Data</title>
    <style>
        body {
            font-family: 'Arial', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #1c1c1c; /* Dark background color */
            color: #e0e0e0; /* Light text color for contrast */
        }
        .container {
            max-width: 800px;
            margin: 20px auto;
            background: #2c2c2c; /* Darker container background */
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }
        h1 {
            text-align: center;
            color: #f5f5f5; /* Slightly lighter text for headings */
            font-size: 24px;
            margin-bottom: 20px;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }
        table, th, td {
            border: 1px solid #444; /* Darker border color */
        }
        th, td {
            padding: 12px;
            text-align: left;
        }
        th {
            background-color: #333; /* Dark background for headers */
            color: #f5f5f5; /* Light text color for headers */
        }
        tr:nth-child(even) {
            background-color: #3a3a3a; /* Alternating row color */
        }
        tr:nth-child(odd) {
            background-color: #2c2c2c; /* Dark background for odd rows */
        }
        tr:hover {
            background-color: #444; /* Highlight row on hover */
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>Online Map: <?php echo htmlspecialchars($playerData['map']); ?></h1>
        <table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Score</th>
                </tr>
            </thead>
            <tbody>
                <?php foreach ($playerData['players'] as $playerName => $playerInfo): ?>
                <tr>
                    <td><?php echo htmlspecialchars($playerName); ?></td>
                    <td><?php echo htmlspecialchars($playerInfo['score']); ?></td>
                </tr>
                <?php endforeach; ?>
            </tbody>
        </table>
    </div>
</body>
</html>
