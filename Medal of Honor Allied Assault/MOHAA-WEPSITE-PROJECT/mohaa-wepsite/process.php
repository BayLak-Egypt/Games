<?php

// Check if data is submitted through POST method
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Check if the form submission is for login
    if (isset($_POST["username"]) && isset($_POST["password"]) && isset($_POST["login"])) {
        // Handle login process
        $username = $_POST["username"];
        $password = $_POST["password"];

        // Here you can perform validation and authentication
        // For demonstration purposes, let's just print the submitted data
        echo "Login Form Submitted <br>";
        echo "Username: " . $username . "<br>";
        echo "Password: " . $password . "<br>";
    }

    // Check if the form submission is for registration
    if (isset($_POST["username"]) && isset($_POST["password"]) && isset($_POST["register"])) {
        // Handle registration process
        $username = $_POST["username"];
        $password = $_POST["password"];

        // Here you can perform registration process and database operations
        // For demonstration purposes, let's just print the submitted data
        echo "Registration Form Submitted <br>";
        echo "Username: " . $username . "<br>";
        echo "Password: " . $password . "<br>";
    }
}

?>
