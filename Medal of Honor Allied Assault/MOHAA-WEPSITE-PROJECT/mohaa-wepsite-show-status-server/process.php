<?php

function sendRconCommand($command, $serverIp, $password, $serverPort)
{
    $socket = fsockopen("udp://$serverIp", (int)$serverPort, $errno, $errstr, 2);
    
    if (!$socket) {
        return "Network error: $errstr ($errno)";
    }

    $rconCommand = "\xFF\xFF\xFF\xFF\x02rcon $password $command";
    
    fwrite($socket, $rconCommand);

    stream_set_timeout($socket, 1);
    $response = fread($socket, 4096);

    fclose($socket);

    return $response;
}

function isValidResponse($response)
{
    return strpos($response, 'map:') !== false && strpos($response, 'num score ping name') !== false;
}

function savePlayerDataToFile($response)
{
    $filePath = 'player_data.txt';
    $existingData = [];

    if (file_exists($filePath)) {
        $existingData = parsePlayerData(file_get_contents($filePath));
    }

    $newData = parsePlayerData($response);

    foreach ($newData as $playerName => $stats) {
        if (isset($existingData[$playerName])) {
            $existingData[$playerName]['score'] += $stats['score'];
        } else {
            $existingData[$playerName] = $stats;
        }
    }

    $content = generatePlayerDataContent($existingData);
    if (file_put_contents($filePath, $content) !== false) {
        // Removed the message about saving player data
    } else {
        echo "<div class='message error'>Error saving data.</div>";
    }
}

function parsePlayerData($data)
{
    $players = [];
    preg_match_all('/(\d+)\s+(\d+)\s+(\d+)\s+(.+)/', $data, $matches, PREG_SET_ORDER);
    foreach ($matches as $match) {
        $score = (int)$match[1];
        $playerName = trim($match[4]);

        $players[$playerName] = [
            'score' => $score
        ];
    }

    return $players;
}

function generatePlayerDataContent($playerData)
{
    $content = "";
    foreach ($playerData as $playerName => $stats) {
        $content .= "{$stats['score']} 0 $playerName\n";
    }
    return $content;
}

function displayPlayerData()
{
    $filePath = 'player_data.txt';

    if (file_exists($filePath)) {
        $data = file_get_contents($filePath);

        preg_match('/map:\s+(\w+)/', $data, $mapMatch);
        $mapName = $mapMatch[1] ?? 'Unknown';

        echo "<h2>Map: $mapName</h2>";

        echo "<table>
                <thead>
                    <tr>
                        <th>Player Name</th>
                        <th>Score</th>
                    </tr>
                </thead>
                <tbody>";

        $players = parsePlayerData($data);
        foreach ($players as $playerName => $stats) {
            $score = $stats['score'];

            echo "<tr>
                    <td>$playerName</td>
                    <td>$score</td>
                </tr>";
        }

        echo "</tbody>
              </table>";
    } else {
        echo "<div class='message info'>No player data available.</div>";
    }
}

// IP ??????? ????????
$ip = "192.168.1.10";
$port = 12203;

$command = "status";
$password = "test123"; // ???? ?????? ???????

$response = sendRconCommand($command, $ip, $password, $port);

if (isValidResponse($response)) {
    savePlayerDataToFile($response);
} elseif (strpos($response, "Bad rconpassword") !== false) {
    echo "<div class='message error'>Invalid RCON password: $password</div>";
} else {
    echo "<div class='message error'>Unexpected response.</div>";
}

displayPlayerData();

?>
