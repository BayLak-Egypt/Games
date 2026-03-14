<?php
session_start();

// التحقق من تسجيل الدخول
if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

// تأكيد أنه تم تمرير معرف العنصر لحذفه
if (!isset($_GET['id'])) {
    echo "معرف العنصر المراد حذفه غير محدد.";
    exit();
}

// معرف العنصر المراد حذفه
$item_id = $_GET['id'];

// اسم المستخدم الحالي
$currentUsername = $_SESSION['username'];

// مصدر البيانات للعناصر
$dataFiles = [
    "Data/data-http-weapons",
    "Data/data-http-tools",
    "Data/data-http-skins",
    "Data/data-http-maps",
    "Data/data-http-mods"
];

$deleted = false;

// حذف العنصر من كل مصدر بيانات
foreach ($dataFiles as $file) {
    if (file_exists($file)) {
        // قراءة البيانات من الملف
        $fileData = file_get_contents($file);
        if ($fileData !== false) {
            // تحويل البيانات إلى مصفوفة من الأسطر
            $rows = explode("\n", trim($fileData));
            
            // البحث عن العنصر وحذفه إذا تم العثور عليه
            foreach ($rows as $key => $row) {
                $rowData = explode(" - ", $row);
                // التحقق من تطابق اسم المستخدم مع المستخدم الحالي
                if (isset($rowData[1]) && $rowData[1] === $currentUsername && isset($rowData[3]) && $rowData[3] === $item_id) {
                    unset($rows[$key]); // حذف العنصر
                    $deleted = true;
                    break; // توقف عن البحث بمجرد العثور على العنصر
                }
            }
            
            // كتابة البيانات المحدثة إلى الملف
            file_put_contents($file, implode("\n", $rows));
        } else {
            echo "فشل في قراءة البيانات من الملف: $file<br>";
        }
    } else {
        echo "الملف غير موجود: $file<br>";
    }
}

?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Delete Data</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
        }
        h2 {
            color: #333;
        }
        p {
            color: #666;
        }
        .button {
            display: inline-block;
            padding: 10px 20px;
            background-color: #f44336;
            color: white;
            text-decoration: none;
            border-radius: 5px;
        }
        .button:hover {
            background-color: #d32f2f;
        }
    </style>
</head>
<body>

<div class="container">
    <?php if ($deleted): ?>
        <h2>تم حذف البيانات بنجاح</h2>
        <p>تم حذف البيانات بنجاح. يمكنك الآن العودة إلى <a href="profile.php?page=uploaded" class="button">صفحة الملف الشخصي</a>.</p>
    <?php else: ?>
        <h2>خطأ في الحذف</h2>
        <p>حدث خطأ أثناء محاولة حذف البيانات. يرجى المحاولة مرة أخرى لاحقًا.</p>
    <?php endif; ?>
</div>

</body>
</html>
