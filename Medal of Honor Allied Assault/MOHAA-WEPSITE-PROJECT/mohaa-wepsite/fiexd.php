<?php

// اسم الملف
$filename = "Data/data-http-maps";

// قراءة المحتوى الخط بخط وحفظه في مصفوفة
$lines = file($filename, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

// مصفوفة لتخزين الصفوف المجمعة
$mergedLines = [];

// المتغير لتخزين آخر تاريخ ووقت
$lastDateTime = null;

// كتابة الصفوف المجمعة
foreach ($lines as $line) {
    // الحصول على التاريخ والوقت من الصف الحالي
    $currentDateTime = substr($line, 0, 19);

    // الفصل إذا كان التاريخ والوقت مختلفين عن الصف السابق
    if ($currentDateTime !== $lastDateTime) {
        // إذا لم يكن هذا أول صف، فقم بكتابة سطر فارغ للفصل
        if ($lastDateTime !== null) {
            $mergedLines[] = ''; // سطر فارغ
        }
        // حفظ التاريخ والوقت الحالي للاستخدام في الصفوف القادمة
        $lastDateTime = $currentDateTime;
    }
    
    // كتابة الصف
    $mergedLines[] = $line;
}

// فتح الملف للكتابة
$file = fopen($filename, "w") or die("Unable to open file!");

// كتابة الصفوف المجمعة في الملف مع إضافة سطر فارغ بينهم
fwrite($file, implode(PHP_EOL, $mergedLines));

// إغلاق الملف بعد الانتهاء من الكتابة
fclose($file);

?>
