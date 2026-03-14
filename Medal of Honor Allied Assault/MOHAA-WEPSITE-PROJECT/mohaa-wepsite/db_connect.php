<?php
$servername = "localhost";
$username = "root";
$password = "";
$database = "my_database";

// إنشاء اتصال
$conn = mysqli_connect($servername, $username, $password, $database);

// التحقق من الاتصال
if (!$conn) {
    die("Connection failed: " . mysqli_connect_error());
}
?>
