<?php
session_start();

// التحقق من تسجيل الدخول
if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

$currentUsername = $_SESSION['username'];

// التحقق من وجود معرف الملف في العنوان URL
$filename = isset($_GET['id']) ? $_GET['id'] : null;
if ($filename === null) {
    echo "File ID is missing.";
    exit();
}

// قائمة بمسارات ملفات البيانات
$dataFiles = [
    "Data/data-http-weapons",
    "Data/data-http-tools",
    "Data/data-http-skins",
    "Data/data-http-maps",
    "Data/data-http-mods"
];

// البحث عن مسار الملف المناسب بناءً على معرف الملف
$dataFilePath = null;
$selectedRow = null;
foreach ($dataFiles as $file) {
    if (file_exists($file)) {
        $fileData = file_get_contents($file);
        if ($fileData !== false) {
            $rows = explode("\n", trim($fileData));
            foreach ($rows as $row) {
                $rowData = explode(" - ", $row);
                if (count($rowData) >= 3 && $rowData[2] === $filename) {
                    if ($rowData[1] !== $currentUsername) {
                        echo "<div style='color: red; text-align: center;'>You do not have permission to edit this entry.</div>";
                        exit();
                    }
                    $dataFilePath = $file;
                    $selectedRow = $rowData;
                    break 2; // الخروج من الحلقتين foreach و foreach
                }
            }
        } else {
            echo "Failed to read data from file: $file<br>";
        }
    } else {
        echo "File not found: $file<br>";
    }
}

// التحقق مما إذا كان تم العثور على مسار الملف
if ($dataFilePath === null || $selectedRow === null) {
    echo "<div style='color: red; text-align: center;'>File or row not found.</div><br>";
    if ($dataFilePath === null) {
        echo "<div style='color: red; text-align: center;'>Could not find the file containing the row.</div><br>";
    }
    if ($selectedRow === null) {
        echo "<div style='color: red; text-align: center;'>Could not find the row in the file.</div><br>";
    }
    exit();
}
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Edit Row</title>
    <style>
        body {
            background-color: #222;
            color: #fff;
            font-family: Arial, sans-serif;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #333;
            border-radius: 5px;
        }

        input[type="text"],
        input[type="file"],
        button {
            width: 100%;
            padding: 10px;
            margin-bottom: 10px;
            border: none;
            border-radius: 3px;
        }

        button {
            background-color: #4CAF50;
            color: white;
            cursor: pointer;
        }

        button:hover {
            background-color: #45a049;
        }

        #currentImage {
            max-width: 200px;
            margin-top: 10px;
            display: block;
        }

        .error-message {
            color: red;
            text-align: center;
            margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <div class="container">
        <h2>Edit <?= htmlspecialchars($selectedRow[2], ENT_QUOTES) ?></h2>
        <form action="UpdateData.php" method="post" enctype="multipart/form-data" onsubmit="return validateForm()">
            <input type="hidden" name="original_filename" value="<?= $filename ?>">
            <input type="hidden" name="dataFilePath" value="<?= $dataFilePath ?>">
            <input type="hidden" name="current_image" value="<?= htmlspecialchars($selectedRow[5], ENT_QUOTES) ?>">
            <label for="filename">Filename:</label>
            <input type="text" name="filename" id="filename" value="<?= htmlspecialchars($selectedRow[2], ENT_QUOTES) ?>" maxlength="30"><br>
            <label for="description">Description:</label>
            <input type="text" name="description" id="description" value="<?= htmlspecialchars($selectedRow[3], ENT_QUOTES) ?>" maxlength="50"><br>
            <label for="link">Link:</label>
            <input type="text" name="link" id="link" value="<?= htmlspecialchars($selectedRow[4], ENT_QUOTES) ?>" maxlength="180"><br>
            <label for="new_image">Upload New Image:</label>
<input type="file" name="new_image" id="new_image" accept="image/*"><br>
<?php if (!empty($selectedRow[5])): ?>
<div>Current Image:</div>
<img src="<?= htmlspecialchars($selectedRow[5]) ?>" alt="Current Image" id="currentImage"><br>
<?php endif; ?>
<button type="submit">Save Changes</button>
</form>
</div>
<script>
    function validateForm() {
        var link = document.getElementById("link").value;
        if (!link.endsWith(".pk3/file")) {
            alert("The link must end with '.pk3/file'. Please provide a valid link.");
            return false;
        }
        if (link.indexOf("mediafire.com") === -1) {
            alert("The link must be from Mediafire. Please provide a valid Mediafire link.");
            return false;
        }
        return true;
    }

    document.querySelector('input[name="new_image"]').addEventListener('change', function(event) {
        var file = event.target.files[0];
        if (file && file.type.startsWith('image/')) {
            var reader = new FileReader();
            reader.onload = function(event) {
                var currentImage = document.querySelector('#currentImage');
                if (currentImage) {
                    currentImage.src = event.target.result;
                    document.querySelector('input[name="current_image"]').value = event.target.result;
                }
            };
            reader.readAsDataURL(file);
        }
    });
</script>
</body>
</html>