<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>رفع ملف جديد</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #1e1e1e; /* لون خلفية الصفحة الداكن */
            color: #fff; /* لون النص */
        }

        form {
            max-width: 500px;
            margin: 0 auto;
            background-color: #333; /* لون خلفية العنصر الرئيسي */
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }

        label {
            display: block;
            margin-bottom: 10px;
            color: #fff; /* لون النص */
        }

        input[type="text"], input[type="file"] {
            width: 100%;
            padding: 10px;
            margin-bottom: 15px;
            border-radius: 5px;
            border: 1px solid #666; /* لون الحدود */
            background-color: #444; /* لون خلفية الحقل */
            color: #fff; /* لون النص */
        }

        input[type="submit"] {
            background-color: #007bff;
            color: #fff;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
            border-radius: 5px;
            transition: background-color 0.3s ease;
        }

        input[type="submit"]:hover {
            background-color: #0056b3;
        }

        input[type="file"] {
            display: none; /* إخفاء زر التحميل الافتراضي */
        }

        .custom-file-upload {
            border: 1px solid #ccc;
            display: inline-block;
            padding: 6px 12px;
            cursor: pointer;
            background-color: #007bff;
            color: #fff;
            border-radius: 5px;
        }
    </style>
</head>
<body>
    <form action="upload_handler.php" method="post" enctype="multipart/form-data">
        <label for="file" class="custom-file-upload">اختر ملف</label>
        <input type="file" name="file" id="file" required>
        <label for="description">وصف الملف:</label>
        <input type="text" name="description" id="description" required>
        <input type="submit" name="submit" value="رفع الملف">
    </form>
</body>
</html>
