<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mods</title>
    <style>
       body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 40px;
            background-color: #1e1e1e; /* لون خلفية الصفحة الداكن */
            color: #fff; /* لون النص */
        }

        .container {
            max-width: 1600px;
            margin: 0 auto;
            background-color: #000; /* لون خلفية العنصر الرئيسي */
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }


        ul {
            padding: 0;
            margin: 0;
            list-style-type: none;
        }

        li {
            margin-bottom: 18px;
            padding: 20px;
            background-color: #000; /* لون خلفية العناصر */
            border-radius: 40px;
            border: 6px solid #444; /* لون حدود العناصر */
        }

        a {
            color: #007bff;
            text-decoration: none;
        }

        a:hover {
            text-decoration: underline;
        }

        a.selected {
            background-color: #007bff; /* لون خلفية العنصر المحدد */
            color: #fff;
            border-color: #007bff; /* لون حدود العنصر المحدد */
        }
        .thumbnail {
            width: 160px;
            height: 90px;
            display: block;
            margin-bottom: 20px;
            border-radius: 8px;
            cursor: pointer;
        }
        .download-button {
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            text-decoration: none;
            border-radius: 15px;
            display: inline-flex;
            align-items: center;
            transition: background-color 0.3s;
            margin-bottom: -90px; /* Added margin for spacing */
        }
        .download-button img {
            margin-right: 8px; /* Space between icon and text */
            width: 20px; /* Set the width of the icon */
            height: auto; /* Maintain aspect ratio */
        }
        .download-button:hover {
            background-color: #0056b3;
        }
        .error {
            color: red;
            font-weight: bold;
        }
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.9);
        }
        .modal-content {
            margin: 10% auto;
            display: block;
            max-width: 80%;
        }
        .close {
            position: absolute;
            top: 15px;
            right: 35px;
            color: #fff;
            font-size: 40px;
            font-weight: bold;
            cursor: pointer;
        }
        /* Style for search form */
        .search-form {
            margin-bottom: 20px;
            text-align: center;
        }
        .search-input {
            padding: 10px;
            width: 60%;
            border-radius: 5px;
            border: 1px solid #ccc;
            font-size: 16px;
        }
        .search-button {
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            border: none;
            border-radius: 5px;
            font-size: 16px;
            cursor: pointer;
            transition: background-color 0.3s;
        }
        .search-button:hover {
            background-color: #0056b3;
        }
        
        tst {
            display: flex;
            justify-content: center;
            padding: 2.5% 5%;
        }
        
        tst img {
            width: 180px;
            height: 36px;
            cursor: pointer;
        }
        
        tst ul {
            display: flex;
            justify-content: center;
            flex-wrap: wrap;
            flex: 1;
            padding: 0;
            margin: 0;
        }
        
        tst ul li {
            display: inline-block;
            padding: 5px 15px;
        }
        
        tst ul li a {
            color: white;
            font-size: 16px;
        }
        
        tst ul li a:hover {
            color: #c50529;
        }
        
        tst ul li a::after {
            content: "";
            width: 0;
            height: 3px;
            display: block;
            transition: 0.6s;
            background-color: #c50529;
        }
        
        tst ul li a:hover::after {
            width: 100%;
        }
        
        .banner {
            width: 100%;
            position: relative;
            height: 300px;
            overflow: hidden;
        }
        
        .banner-img {
            width: 100%;
            height: auto;
            display: block;
        }
        
        .banner-overlay {
            transform: translate(25%, -60%);
            color: #fff;
        }


    </style>

<div class="banner">
  <img src="images/bannar.jpg" alt="Banner" class="banner-img">
  <div class="banner-overlay">
    <tst>
        <ul>
          <li><a href="maps.php">Maps</a></li>
          <li><a href="mods.php">Mods</a></li>
          <li><a href="skins.php">Skins</a></li>
          <li><a href="weapons.php">Weapons</a></li>
          <li><a href="tools.php">Tools</a></li>
          <li><a href="http://modelviewer.appelpitje.be/">3D Models</a></li>
	      <li><a href="servers.php">Servers</a></li>
	      <li><a href="profile.php">Profile</a></li>
        </ul>
      </tst>
  </div>
</div>
</head>
<body>
    <div class="container">
        <!-- Search Form -->
        <form method="get" action="" class="search-form">
            <input type="text" name="search" class="search-input" >
            <button type="submit" class="search-button">Search</button>
        </form>

<?php
// قراءة ملف البيانات
$file = 'Data/data-http-mods';
$data = file_get_contents($file);
$rows = explode("\n", $data);

$searchResults = array(); // Array to store search results

if (isset($_GET['search'])) {
    $searchTerm = $_GET['search'];
    foreach ($rows as $row) {
        $columns = explode(" - ", $row);
        if (count($columns) >= 6) {
            $dateTime = htmlspecialchars($columns[0]);
            $username = htmlspecialchars($columns[1]);
            $filename = htmlspecialchars($columns[2]);
            $description = htmlspecialchars($columns[3]);
            $downloadLink = htmlspecialchars($columns[4]);
            $path = htmlspecialchars($columns[5]);

            // Check if search term exists in filename or description
            if (stripos($filename, $searchTerm) !== false || stripos($description, $searchTerm) !== false) {
                // Store matching results
                $searchResults[] = array(
                    'dateTime' => $dateTime,
                    'username' => $username,
                    'filename' => $filename,
                    'description' => $description,
                    'downloadLink' => $downloadLink,
                    'path' => $path
                );
            }
        }
    }
}

foreach ($searchResults as $result) {
    echo "<div style='display:flex; align-items:center; border-radius: 8px; border: 2px solid #444; padding: 10px; margin-bottom: 20px;'>";
    echo "<img src='{$result['path']}' alt='Uploaded Image' class='thumbnail' style='border-radius: 8px; width: 120px; height: auto;' onclick='openModal(this.src)'>";
    echo "<div style='margin-left:10px; flex: 1;'>"; // Add margin to separate image and text and make it occupy remaining space
    echo "<p>Date/Time: {$result['dateTime']}</p>";
    echo "<p>Filename: {$result['filename']}</p>";
    echo "<p>Description: {$result['description']}</p>";
    echo "<p>Username: {$result['username']}</p>";
    echo "</div>"; // Close the div for text
    echo "<a href='{$result['downloadLink']}' class='download-button' target='_blank'><img src='images/mediafire.png' alt='MediaFire Icon' style='width: 20px; height: auto;'>Download File</a>";
    echo "</div>"; // Close the div for flex layout
}

// Display all files if no search term is provided or if no results found
if (empty($searchResults)) {
    foreach ($rows as $row) {
        $columns = explode(" - ", $row);
        if (count($columns) >= 6) {
            $dateTime = htmlspecialchars($columns[0]);
            $username = htmlspecialchars($columns[1]);
            $filename = htmlspecialchars($columns[2]);
            $description = htmlspecialchars($columns[3]);
            $downloadLink = htmlspecialchars($columns[4]);
            $path = htmlspecialchars($columns[5]);

            echo "<div style='display:flex; align-items:center; border-radius: 8px; border: 2px solid #444; padding: 10px; margin-bottom: 20px;'>";
            echo "<img src='$path' alt='Uploaded Image' class='thumbnail' style='border-radius: 8px; width: 120px; height: auto;' onclick='openModal(this.src)'>";
            echo "<div style='margin-left:10px; flex: 1;'>"; // Add margin to separate image and text and make it occupy remaining space
            echo "<p>Date: $dateTime</p>";
            echo "<p>Filename: $filename</p>";
            echo "<p>Description: $description</p>";
            echo "<p>By: $username</p>";
            echo "</div>"; // Close the div for text
            echo "<a href='$downloadLink' class='download-button' target='_blank'><img src='images/mediafire.png' alt='MediaFire Icon' style='width: 20px; height: auto;'>Download File</a>";
            echo "</div>"; // Close the div for flex layout
        }
    }
}


?> 

    </div>
    <!-- The Modal -->
    <div id="myModal" class="modal">
        <span class="close" onclick="closeModal()">&times;</span>
        <img class="modal-content" id="img01">
    </div>

    <script>
               // JavaScript functions for modal
        function openModal(src) {
                        var modal = document.getElementById("myModal");
            var modalImg = document.getElementById("img01");
            modal.style.display = "block";
            modalImg.src = src;
        }

        function closeModal() {
            var modal = document.getElementById("myModal");
            modal.style.display = "none";
        }
    </script>
</body>
</html>
