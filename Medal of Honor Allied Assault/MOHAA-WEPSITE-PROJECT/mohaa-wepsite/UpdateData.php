<?php
session_start();

// التحقق من تسجيل الدخول
if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

$currentUsername = $_SESSION['username'];

// استلام البيانات من النموذج
$originalFilename = isset($_POST['original_filename']) ? $_POST['original_filename'] : null;
$dataFilePath = isset($_POST['dataFilePath']) ? $_POST['dataFilePath'] : null;
$currentImage = isset($_POST['current_image']) ? $_POST['current_image'] : null;
$filename = isset($_POST['filename']) ? $_POST['filename'] : null;
$description = isset($_POST['description']) ? $_POST['description'] : null;
$link = isset($_POST['link']) ? $_POST['link'] : null;
$newImage = isset($_FILES['new_image']) ? $_FILES['new_image'] : null;

// التحقق من توفر المسار ومعرف الملف الأصلي
if ($dataFilePath === null || $originalFilename === null) {
    echo "Data file path or original filename is missing.";
    exit();
}

// التحقق من تعديل المستخدم الحالي
$fileData = file_get_contents($dataFilePath);
$rows = explode("\n", trim($fileData));
$newRows = [];
$fileModified = false;
foreach ($rows as $row) {
    $rowData = explode(" - ", $row);
    if (count($rowData) >= 3 && $rowData[2] === $originalFilename && $rowData[1] === $currentUsername) {
        // التحقق من وجود صورة بنفس الاسم
        if ($newImage['error'] === UPLOAD_ERR_OK) {
            $imagePath = 'uploads/' . basename($newImage['name']);
            $filenameParts = pathinfo($imagePath);
            $fileExtension = isset($filenameParts['extension']) ? '.' . $filenameParts['extension'] : '';
            $baseFilename = isset($filenameParts['filename']) ? $filenameParts['filename'] : '';

            // إذا كان الملف موجودًا، أضف رقمًا إلى اسم الملف الجديد
            $i = 1;
            while (file_exists($imagePath)) {
                $imagePath = 'uploads/' . $baseFilename . '_' . $i . $fileExtension;
                $i++;
            }

            // حذف الصورة القديمة إذا كانت موجودة وليست الصورة الافتراضية
            if ($rowData[5] !== 'uploads/mohaa.jpg' && file_exists($rowData[5])) {
                unlink($rowData[5]);
            }

            // حفظ الصورة بالاسم الجديد
            move_uploaded_file($newImage['tmp_name'], $imagePath);
            $rowData[5] = $imagePath;
        }
        // تحديث البيانات
        $rowData[2] = $filename;
        $rowData[3] = $description;
        $rowData[4] = $link;
        $newRows[] = implode(" - ", $rowData);
        $fileModified = true;
    } else {
        $newRows[] = $row;
    }
}

// إذا تم تعديل الملف، قم بحفظ البيانات الجديدة
if ($fileModified) {
    $newData = implode("\n", $newRows);
    file_put_contents($dataFilePath, $newData);
}

// إعادة التوجيه إلى الصفحة السابقة
header("Location: profile.php?success=true");
exit();
?>
