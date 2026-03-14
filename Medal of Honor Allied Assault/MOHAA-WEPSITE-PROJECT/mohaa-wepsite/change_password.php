<?php
session_start();

// Include database connection
include_once 'db_connect.php';

// Check if the form is submitted
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Get current username from session
    $username = $_SESSION['username'];

    // Get old, new, and confirm passwords from the form
    $oldPassword = $_POST['oldPassword'];
    $newPassword = $_POST['newPassword'];
    $confirmPassword = $_POST['confirmPassword'];

    // Check if the old password matches the one in the database
    $sql = "SELECT * FROM users WHERE username='$username' AND password='$oldPassword'";
    $result = mysqli_query($conn, $sql);

    // If the old password matches
    if (mysqli_num_rows($result) == 1) {
        // Check if new password matches confirm password
        if ($newPassword === $confirmPassword) {
            // Update password in the database
            $updateSql = "UPDATE users SET password='$newPassword' WHERE username='$username'";
            if (mysqli_query($conn, $updateSql)) {
                // Password updated successfully
                echo "تم تغيير كلمة المرور بنجاح!";
            } else {
                echo "حدث خطأ أثناء تحديث كلمة المرور: " . mysqli_error($conn);
            }
        } else {
            echo "كلمة المرور الجديدة وتأكيدها غير متطابقتين.";
        }
    } else {
        echo "كلمة المرور القديمة غير صحيحة.";
    }
}
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Change Password</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            text-align: center;
            align-items: center;
            justify-content: center;
            background-color: #1e1e1e; /* لون خلفية الصفحة الداكن */
            color: #fff; /* لون النص */
        }

        .container {
            display: flex;
            max-width: 600px;
            margin: 0 auto;
            background-color: #000; /* لون خلفية العنصر الرئيسي */
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            flex-direction: column;
        }

        .input-group {
            margin-bottom: 20px;
            text-align: right;
        }

        .input-group label {
            margin-bottom: 5px;
            display: block;
            text-align: right;
        }

        .input-group input {
            width: calc(100% - 10px);
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
            text-align: right;
            direction: rtl;
        }

        .button-group {
            display: flex;
            justify-content: space-between;
        }

        .button {
            flex-grow: 1;
            background-color: #007bff;
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
        }

        .button.red {
            background-color: #ff0000;
        }
    </style>
</head>
<body>
    <div class="container">
        <h2>تغيير كلمة المرور</h2>
        <form action="" method="post">
            <div class="input-group">
                <label for="oldPassword">كلمة المرور القديمة:</label>
                <input type="password" id="oldPassword" name="oldPassword" required>
            </div>
            <div class="input-group">
                <label for="newPassword">كلمة المرور الجديدة:</label>
                <input type="password" id="newPassword" name="newPassword" required>
            </div>
            <div class="input-group">
                <label for="confirmPassword">تأكيد كلمة المرور الجديدة:</label>
                <input type="password" id="confirmPassword" name="confirmPassword" required>
            </div>
            <div class="button-group">
                <button type="submit" class="button">تأكيد التغيير</button>
                <a href="logout.php" class="button">تسجيل الخروج</a>
<a href="delete_account.php" class="button red">حذف الحساب</a>
</div>
</form>
</div>

</body>
</html>
