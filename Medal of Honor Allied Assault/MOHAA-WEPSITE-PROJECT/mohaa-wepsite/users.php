<?php
session_start();

// Include database connection file
include_once "db_connect.php";

// استعلام SQL لاختيار جميع المستخدمين
$sql = "SELECT username, authenticated FROM users";
$result = $conn->query($sql);
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>All Users</title>
    <style>
        /* CSS Styles */
        body {
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }

        .container {
            max-width: 600px;
            margin: 50px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        h2 {
            color: #333;
            text-align: center;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th, td {
            padding: 10px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
        }

        /* أيقونة المصادقة */
        .verification-icon {
            width: 20px;
            height: auto;
            margin-left: 5px;
        }
    </style>
</head>
<body>
    <div class="container">
        <h2>All Users</h2>
        <table>
            <thead>
                <tr>
                    <th>Username</th>
                    <th>Authenticated</th>
                </tr>
            </thead>
            <tbody>
                <?php
                // عرض البيانات
                if ($result->num_rows > 0) {
                    while ($row = $result->fetch_assoc()) {
                        echo "<tr>";
                        echo "<td>{$row['username']}</td>";
                        // عرض الأيقونة إذا كان المستخدم موثقًا
                        if ($row['authenticated'] == 1) {
                            echo '<td><img src="images/verification.png" alt="Verification Icon" class="verification-icon"></td>';
                        } else {
                            echo "<td></td>";
                        }
                        echo "</tr>";
                    }
                } else {
                    echo "<tr><td colspan='2'>No users found.</td></tr>";
                }
                ?>
            </tbody>
        </table>
    </div>
</body>
</html>

<?php
// إغلاق اتصال قاعدة البيانات
$conn->close();
?>
