<?php
session_start();

// Include database connection file
include_once "db_connect.php";

if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

$username = $_SESSION['username'];

$stmt = $conn->prepare("SELECT authenticated FROM users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();
$stmt->bind_result($authenticated);
$stmt->fetch();
$stmt->close();

$logout_link = '<a href="logout.php" class="logout-btn">Logout</a>';

// تعيين الألوان الافتراضية
$default_color = "black";

if ($authenticated == 1) {
    // تعيين متغير لاحتواء رمز الأيقونة
    $verification_icon = '<img src="images/verification.png" alt="Verification Icon" style="width: 20px; height: auto; margin-left: 5px;">';
    // تعيين قيمة لاسم المستخدم مع الأيقونة
    $username_with_icon = $username . $verification_icon;
    // تحديد ألوان النص
    $username_style = "color: blue;"; // استخدم لون أزرق إذا كان المستخدم موثقاً
} else {
    // إذا كانت الحالة غير موثقة، استخدم الألوان الافتراضية
    $username_with_icon = $username;
    $username_style = "color: $default_color;"; // استخدم اللون الافتراضي للنص
}
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Profile</title>
    <style>
        /* CSS Styles */
       body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #1e1e1e; /* لون خلفية الصفحة الداكن */
            color: #fff; /* لون النص */
        }
        
        .container {
            max-width: 1600px;
            margin: 0 auto;
            background-color: #000; /* لون خلفية العنصر الرئيسي */
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }
        /* Apply animation to username */
        @keyframes changeColor {
            0% { color: blue; }
            33% { color: green; }
            66% { color: red; }
            100% { color: blue; }
        }

        .username_animation {
            animation: changeColor 5s infinite;
        }

        /* Logout button style */
        .logout-btn {
            text-decoration: none;
            color: #333;
            border: 1px solid #333;
            padding: 5px 10px;
            border-radius: 5px;
            background-color: #fff;
            transition: background-color 0.3s ease;
        }

        .logout-btn:hover {
            background-color: #333;
            color: #fff;
        }
    </style>
</head>
<body>

<div class="container">
    <h2>Welcome, <?php echo '<span class="username_animation" style="' . $username_style . '">' . $username_with_icon . '</span>'; ?>!</h2>
</div>

</body>
</html>

