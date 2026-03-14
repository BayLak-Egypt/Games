<?php
session_start();

// التحقق من تسجيل الدخول
if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

// المستخدم الحالي
$currentUsername = $_SESSION['username'];

// قائمة بمسارات ملفات البيانات
$dataFiles = [
    "Data/data-http-weapons",
    "Data/data-http-tools",
    "Data/data-http-skins",
    "Data/data-http-maps",
    "Data/data-http-mods"
];

$dataRows = [];

// قراءة البيانات من كل ملف
foreach ($dataFiles as $file) {
    if (file_exists($file)) {
        $fileData = file_get_contents($file);
        if ($fileData !== false) {
            $rows = explode("\n", trim($fileData));
            foreach ($rows as $row) {
                $rowData = explode(" - ", $row);
                // التحقق من تطابق اسم المستخدم في المصدر الأول
                if (count($rowData) >= 6 && $rowData[1] === $currentUsername) {
                    if ($rowData[1] === $currentUsername) {
                        $dataRows[] = $row;
                    }
                }
            }
        } else {
            echo "Failed to read data from file: $file<br>";
        }
    } else {
        echo "File not found: $file<br>";
    }
}

?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>View Data</title>
    <style>
        table {
            width: 100%;
            border-collapse: collapse;
        }
        th, td {
            padding: 8px;
            text-align: left;
            border-bottom: 1px solid #fff;
        }
        th {
            background-color: #000000;
        }
        tr:hover {
            background-color: #F08080;
        }
        a {
            text-decoration: none;
            color: blue;
        }
        a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <?php
    // عرض البيانات في جدول
    if (!empty($dataRows)) {
        echo "<table>";
        echo "<tr><th>Name</th><th>Description</th><th>Link</th><th>Edit</th><th>Delete</th></tr>";
        foreach ($dataRows as $row) {
            $rowData = explode(" - ", $row);
            echo "<tr>";
            echo "<td>" . htmlspecialchars($rowData[2]) . "</td>";
            echo "<td>" . htmlspecialchars($rowData[3]) . "</td>";
            echo "<td><a href='" . htmlspecialchars($rowData[4]) . "' target='_blank'>" . htmlspecialchars($rowData[4]) . "</a></td>";
            echo "<td><a href='EditData.php?id=" . htmlspecialchars($rowData[2]) . "'>Edit</a></td>";
            echo "<td><a href='DeleteData.php?id=" . htmlspecialchars($rowData[3]) . "'>Delete</a></td>";
            echo "</tr>";
        }
        echo "</table>";
    } else {
        echo "No data available.";
    }
    ?>
</body>
</html>