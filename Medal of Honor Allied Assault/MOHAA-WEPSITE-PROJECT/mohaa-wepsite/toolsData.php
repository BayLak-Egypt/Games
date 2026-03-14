<?php
session_start();

// تضمين ملف الاتصال بقاعدة البيانات
include_once "db_connect.php";

// إعادة التوجيه إلى صفحة تسجيل الدخول إذا لم يتم تسجيل الدخول
if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

// الحصول على اسم المستخدم من الجلسة
$username = $_SESSION['username'];

$logout_link = '<a href="logout.php" class="logout-btn">تسجيل الخروج</a>';

// تحديد الألوان الافتراضية
$default_color = "black";
$verification_icon = '';
$username_style = "color: $default_color;";

// التحقق مما إذا تم إرسال النموذج
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $name = $_POST["name"];
    $description = $_POST["description"];
    $link = $_POST["link"];

    // التحقق من صحة رابط MediaFire
    if (strpos($link, "mediafire.com") === false || strpos($link, ".pk3/file") === false) {
        echo "يُسمح فقط بروابط MediaFire التي تحتوي على '.pk3/file'.";
    } else {
        // قراءة البيانات المحفوظة سابقاً من الملف
        $fileData = file_get_contents("Data/data-http-tools");

        // التحقق مما إذا كان الاسم أو الوصف أو الرابط مستخدمًا بالفعل
        if (strpos($fileData, $name) !== false) {
            echo "الاسم مستخدم بالفعل.";
        } elseif (strpos($fileData, $description) !== false) {
            echo "الوصف مستخدم بالفعل.";
        } elseif (strpos($fileData, $link) !== false) {
            echo "الرابط مستخدم بالفعل.";
        } else {
            if (!empty($name) && !empty($description) && !empty($link)) {
                $targetPath = "uploads/mohaa.jpg";
                $imagePath = '';
                if ($_FILES["image"]["error"] != 4) {
                    $imagePath = $_FILES["image"]["tmp_name"];
                    $imageFileType = strtolower(pathinfo($_FILES["image"]["name"], PATHINFO_EXTENSION));
                    $allowedTypes = ['jpg', 'jpeg', 'png', 'gif'];

                    if (in_array($imageFileType, $allowedTypes)) {
                        $targetPath = "uploads/" . basename($_FILES["image"]["name"]);
                        move_uploaded_file($imagePath, $targetPath);
                    } else {
                        echo "يُسمح فقط بملفات الصور (jpg, jpeg, png, gif).";
                    }
                }
                $dateTime = date("Y-m-d H:i:s");
                $data = "$dateTime - $username - $name - $description - $link - $targetPath\n\n"; // Add extra newline for empty row

                $max_name_length = 30;
                $max_description_length = 50;
                $max_link_length = 180;

                if (strlen($description) > $max_description_length) {
                    echo "الوصف يجب ألا يتجاوز $max_description_length حرف.";
                } elseif (strlen($name) > $max_name_length) {
                    echo "الاسم يجب ألا يتجاوز $max_name_length حرف.";
                } elseif (strlen($link) > $max_link_length) {
                    echo "الرابط يجب ألا يتجاوز $max_link_length حرف.";
                } else {
                    // Ensure data is saved correctly with an empty row after each entry
                    file_put_contents("Data/data-http-tools", $data, FILE_APPEND | LOCK_EX);
                    header("Location: profile.php?success=true");
                    exit();
                }
            } else {
                echo "يرجى ملء جميع الحقول.";
            }
        }
    }
}
?>
<!DOCTYPE html>
<html lang="ar">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Tools Data</title>
    <style>
        /* CSS Styles */
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #1e1e1e;
            color : #fff;
        }

        .container {
            max-width: 1600px;
            margin: 0 auto;
            background-color: #000;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }

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

        label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
        }

        input[type="text"],
        textarea {
            width: 100%;
            padding: 8px;
            margin-bottom: 10px;
            border: 1px solid #ccc;
            border-radius: 3px;
            box-sizing: border-box;
            background-color: #333; /* لون الخلفية الداكن لحقول الإدخال */
            color: #fff; /* لون النص */
        }

        input[type="file"] {
            margin-bottom: 10px;
        }

        input[type="button"],
        input[type="submit"] {
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            border: none;
            border-radius: 3px;
            cursor: pointer;
        }

        input[type="button"]:hover,
        input[type="submit"]:hover {
            background-color: #0056b3;
        }

        .custom-file-upload {
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            border: none;
            border-radius: 3px;
            cursor: pointer;
        }

        .custom-file-upload:hover {
            background-color: #0056b3;
        }

        select {
            width: 100%;
            padding: 8px;
            border: 1px solid #ccc;
            border-radius: 3px;
            background-color: #fff;
            font-size: 14px;
            color: #333;
            cursor: pointer;
        }

        #imageInput {
            background-color: #333; /* لون الخلفية */
            color: #fff; /* لون النص */
            border: none;
            padding: 10px 20px;
            border-radius: 3px;
            cursor: pointer;
        }

        #imageInput:hover {
            background-color: #555; /* تغيير اللون عند تحويل المؤشر */
        }

        .preview-img {
            max-width: 100px;
            max-height: 100px;
            margin-bottom: 10px;
            display: block;
        }
    </style>
    <script>
        function previewImage(event) {
            var reader = new FileReader();
            reader.onload = function(){
                var output = document.getElementById('preview');
                output.src = reader.result;
                output.style.display = "block";
            };
            reader.readAsDataURL(event.target.files[0]);
        }
    </script>
</head>
<body>

<div class="container">
    <form method="post" action="<?php echo htmlspecialchars($_SERVER["PHP_SELF"]); ?>" enctype="multipart/form-data">
        <label for="mapsInput" style="color: red; font-size: 20px;">Tools</label><br>
        <label for="imageInput">الصورة:</label>
        <input type="file" id="imageInput" name="image" accept="image/*" onchange="previewImage(event)"><br>
        <img id="preview" src="#" alt="معاينة الصورة" class="preview-img" style="display: none;"><br>
        <label for="nameInput">الاسم:</label>
        <input type="text" id="nameInput" name="name" maxlength="30"><br>
        <label for="descriptionInput">الوصف:</label><br>
        <textarea id="descriptionInput" name="description" maxlength="50"></textarea><br>
        <label for="linkInput">الرابط:</label>
        <input type="text" id="linkInput" name="link" maxlength="180"><br>
        <input type="submit" value="متابعة"><br>
    </form>
</div>

</body>
</html>
