<!DOCTYPE html>
<html lang="ar">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>تغيير كلمة المرور</title>
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
            max-width: 1600px;
            margin: 0 auto;
            background-color: #000; /* لون خلفية العنصر الرئيسي */
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }

        .input-group {
            margin-bottom: 20px;
        }

        .input-group label {
            display: block;
            margin-bottom: 5px;
        }

        .input-group input {
            width: calc(100% - 10px);
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
            direction: rtl;
        }

        .button-group {
            margin-top: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center; /* Align items vertically */
        }

        .button {
            flex-grow: 1;
            background-color: #007bff;
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .button.red {
            background-color: #ff5f5f;
        }

        .button:hover {
            background-color: #0056b3;
        }

        /* Style for the delete button */
        .button.delete {
            background-color: #ff0000;
            margin-left: 10px; /* Add space between buttons */
        }

        /* Adjusting button sizes for a cleaner look */
        .button.delete,
        .button-group a {
            padding: 10px 15px; /* Smaller padding */
        }
    </style>
</head>
<body>
    <h2 style="text-align: center;">تغيير كلمة المرور</h2>
    <div class="container">
        <form action="change_password.php" method="post">
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
            <button type="submit" class="button">تأكيد التغيير</button>
        </form>
    </div>
    <div class="button-group">
        <a href="logout.php" class="button">تسجيل الخروج</a>
        <form action="delete_account.php" method="post" onsubmit="return confirmDelete();">
            <input type="submit" value="حذف الحساب" class="button delete">
        </form>
    </div>
    <script>
        function confirmDelete() {
            return confirm("هل أنت متأكد أنك تريد حذف حسابك؟ سيتم حذف جميع ما نشرته.");
        }
    </script>
</body>
</html>
