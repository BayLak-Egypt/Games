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
    $save_location = $_POST["save_location"]; // الحصول على مسار الحفظ من القائمة المنسدلة

    // التحقق من صحة رابط MediaFire
    if (strpos($link, "mediafire.com") === false || strpos($link, ".pk3/file") === false) {
        echo "يُسمح فقط بروابط MediaFire التي تحتوي على '.pk3/file'.";
    } else {
        // تحديد المسار بناءً على الاختيار
        switch ($save_location) {
            case 'weapons':
                $filePath = "Data/data-http-weapons";
                break;
            case 'skins':
                $filePath = "Data/data-http-skins";
                break;
            case 'maps':
                $filePath = "Data/data-http-maps";
                break;
            case 'mods':
                $filePath = "Data/data-http-mods";
                break;
            default:
                $filePath = "Data/data-http-maps";
        }

        // قراءة البيانات المحفوظة سابقاً من الملف
        $fileData = file_get_contents($filePath);

        // التحقق مما إذا كان الاسم أو الوصف أو الرابط مستخدمًا بالفعل
        if (strpos($fileData, $name) !== false) {
            // إذا كان الاسم مستخدم بالفعل، أضف رقمًا له
            $name .= rand(1, 1000);
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
                $data = "\n$dateTime - $username - $name - $description - $link - $targetPath\n"; // إضافة سطر فارغ فوق وتحت كل مدخل


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
                    // التأكد من حفظ البيانات بشكل صحيح مع إضافة سطر فارغ بعد كل مدخل
                    file_put_contents($filePath, $data, FILE_APPEND | LOCK_EX);
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
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Maps Data</title>
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

        .logout-btn {
            text-decoration: none;
            color: #fff;
            border: 1px solid #fff;
            padding: 8px 16px; /* Adjusted padding */
            border-radius: 5px
			            background-color: transparent;
            transition: background-color 0.3s ease;
            margin-right: 10px; /* إضافة هذا الخاصية للزر */
        }

        .logout-btn:hover {
            background-color: #fff;
            color: #000;
        }

        label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #fff;
        }

        input[type="text"],
        textarea,
        input[type="file"],
        select {
            width: calc(100% - 22px); /* Adjusted width */
            padding: 10px;
            margin-bottom: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
            box-sizing: border-box;
            background-color: #333; /* Dark background color for input fields */
            color: #fff; /* Light text color */
        }

        input[type="file"] {
            cursor: pointer;
        }

        input[type="button"],
        input[type="submit"] {
            width: 100%;
            padding: 12px;
            background-color: #ff0000;
            color: #fff;
            border: 1px solid transparent;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.3s ease, border-color 0.3s ease;
            box-sizing: border-box;
            margin-top: -10px;
        }

        input[type="button"]:hover,
        input[type="submit"]:hover {
            background-color: #d60000;
            border-color: #d60000;
        }

        input[type="submit"]:focus {
            outline: none;
        }

        .preview-img {
            max-width: 100px;
            max-height: 100px;
            margin-bottom: 10px;
            display: block;
        }

        select {
            padding: 12px;
            background-color: #333;
            color: #fff;
            cursor: pointer;
        }

        .link-container {
            display: flex;
            align-items: center;
            margin-top: 20px;
        }

        .link-container label {
            margin-right: 10px;
        }

        .link-container input[type="text"] {
            flex: 1;
            margin-right: 10px;
        }

        .link-container input[type="submit"] {
            width: auto;
            padding: 12px 24px;
            margin-right: 10px; /* إضافة هذا الخاصية للزر */
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

        function updateFilePath() {
            const selectElement = document.getElementById('saveLocation');
            const selectedValue = selectElement.options[selectElement.selectedIndex].text;
            document.getElementById('filePathDisplay').innerText = selectedValue;
        }
    </script>
</head>
<body>

<div class="container">
    <form method="post" action="<?php echo htmlspecialchars($_SERVER["PHP_SELF"]); ?>" enctype="multipart/form-data">
        <h1 style="color: red; text-align: center;">Upload</h1>
        <label for="saveLocation">For</label>
        <select id="saveLocation" name="save_location" onchange="updateFilePath()">
            <option value="maps">Maps</option>
            <option value="mods">Mods</option>
            <option value="skins">Skins</option>
            <option value="weapons">Weapons</option>
        </select>
        <label for="imageInput">Image:</label>
        <input type="file" id="imageInput" name="image" accept="image/*" onchange="previewImage(event)">
        <label for="nameInput">Name:</label>
        <input type="text" id="nameInput" name="name" maxlength="30">
        <label for="descriptionInput">Description:</label>
        <textarea id="descriptionInput" name="description" maxlength="50"></textarea>
        <div class="link-container">
            <label for="linkInput">Link:</label>
            <input type="text" id="linkInput" name="link" maxlength="180">
            <input type="submit" value="Save">
        </div>
        <img id="preview" src="#" alt="Image Preview" class="preview-img" style="display: none; margin-top: 10px;">
    </form>
</div>

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

    function updateFilePath() {
        const selectElement = document.getElementById('saveLocation');
        const selectedValue = selectElement.options[selectElement.selectedIndex].text;
        document.getElementById('filePathDisplay').innerText = selectedValue;
    }
</script>

</body>
</html>
