<?php
// Include database connection
include 'db_connect.php';

// Start the session
session_start();

// Check if the user is logged in
if (!isset($_SESSION['username'])) {
    // If not logged in, redirect to the login page
    header("Location: login.php");
    exit;
}

// Function to delete records from a file
function deleteRecordsFromFile($filename, $username) {
    // Open the data file for reading and writing
    $lines = file($filename);

    // Array to store updated lines
    $updated_lines = array();

    // Username to delete
    $username_to_delete = "$username -";

    // Loop through each line in the file
    foreach ($lines as $line) {
        // Check if the line contains the username
        if (strpos($line, $username_to_delete) === false) {
            // If not, add it to the updated lines array
            $updated_lines[] = $line;
        }
    }

    // Rewrite the file with updated lines
    file_put_contents($filename, implode("", $updated_lines));
}

// Process delete account request
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $username = $_SESSION['username'];

    // Delete account query
    $sql = "DELETE FROM users WHERE username='$username'";

    // Execute query
    if (mysqli_query($conn, $sql)) {
        // Account deleted successfully
        echo "Account deleted successfully.";

        // Array of data files to check and delete records from
        $dataFiles = [
            "Data/data-http-weapons",
            "Data/data-http-tools",
            "Data/data-http-skins",
            "Data/data-http-maps",
            "Data/data-http-mods"
        ];

        // Loop through each data file and delete records
        foreach ($dataFiles as $filename) {
            deleteRecordsFromFile($filename, $username);
        }

        // Destroy session and redirect to login page
        session_destroy();
        header("Location: login.php");
        exit;
    } else {
        // Error deleting account
        echo "Error deleting account: " . mysqli_error($conn);
    }
}
?>
