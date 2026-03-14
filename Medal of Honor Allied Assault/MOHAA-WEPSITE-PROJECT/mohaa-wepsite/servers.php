<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Servers AA/SP/BR</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-u+j8AzCz5dUKYfUZ2gqRfTBoOg0rNq6lZZT8tJCNObtwXLx1LpPt4X2jJcuFjOpq5J1q5z36oMn2Q6uxvj7yLw==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 40px;
            background-color: #1e1e1e;
            color: #fff;
        }
        .container {
            max-width: 1600px;
            margin: 0 auto;
            background-color: #000;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            margin-top: 20px; /* تعديل المسافة العلوية */
        }
        ul {
            padding: 0;
            margin: 0;
            list-style-type: none;
        }

        li {
            margin-bottom: 18px;
            padding: 20px;
            background-color: #000; 
            border-radius: 40px;
            border: 6px solid #444; 
        }

        a {
            color: #007bff;
            text-decoration: none;
        }

        a:hover {
            text-decoration: underline;
        }

        a.selected {
            background-color: #007bff; 
            color: #fff;
            border-color: #007bff; 
        }

        .file-icon::before {
            content: "\f15c"; 
            margin-right: 8px;
        }

        .download-icon::before {
            content: "\f019"; 
            margin-left: 8px;
        }

        .image {
            width: 100px;
            height: 100px;
            object-fit: cover;
            border-radius: 8px;
            margin-bottom: 10px;
            cursor: pointer;
        }

        .overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.7);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 999;
            display: none;
        }

        .overlay-content {
            max-width: 80%;
            max-height: 80%;
            text-align: center;
            position: relative;
        }

        .overlay img {
            max-width: 100%;
            max-height: 100%;
            border-radius: 8px;
            cursor: pointer;
        }

        .close-btn {
            position: absolute;
            top: 10px;
            right: 10px;
            color: #fff;
            font-size: 24px;
            cursor: pointer;
        }
        
        .item {
            list-style: none;
            margin-bottom: 20px;
        }

        .item .item-content {
            display: flex;
            align-items: flex-start;
        }

        .item img {
            max-width: 100px;
            margin-right: 20px;
            cursor: pointer;
        }

        .item .details {
            flex-grow: 1;
        }

        .item .details h3 {
            margin: 0 0 5px 0;
        }

        .item .details p {
            margin: 0;
            margin-bottom: 5px;
        }

        .item .download-btn {
            margin-top: auto; 
            display: inline-block;
            text-decoration: none;
            color: #333;
            background-color: #f2f2f2;
            padding: 10px;
            border-radius: 5px;
        }

        .item .download-btn:hover {
            background-color: #ddd;
        }

        tst {
            display: flex;
            padding: 2.5% 5%;
        }
        tst img {
            width: 180px;
            height: 36px;
            cursor: pointer;
        }
        tst ul {
            text-align: right;
            flex: 1;
            padding-top: 0; /* تعديل المسافة العلوية */
            margin-bottom: 0; /* تعديل المسافة السفلية */
        }
        tst ul li {
            display: inline-block;
            padding: 6px 15px;
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
            height: 2px;
            display: block;
            transition: 0.6s;
            background-color: #c50529;
        }
        tst ul li a:hover::after {
            width: 100%;
        }
        .banner1 {
            width: 100%;
            position: relative;
            height: 300px;
            overflow: hidden;
        }

        .banner1-img {
            width: 100%;
            height: auto;
            display: block;
        }

        .banner1-overlay {
            transform: translate(2%, -62%);
            color: #fff;
        }
    </style>
</head>
<body>
    <div class="banner1">
        <img src="images/bannar.jpg" alt="Banner" class="banner1-img">
        <div class="banner1-overlay">
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

    <div class="container">
        <ul>
            <?php
                // تضمين المكتبة simple_html_dom.php
                include("simple_html_dom.php");

                // استرجاع محتوى الصفحة
                $html = file_get_html('https://www.mohaaservers.tk/');
                
                // مصفوفة تحتوي على الفئات التي تريد حذفها
                $classes_to_remove = array('.expander.players', '.tablesorter-header-inner', '.long', '.centerTitle', '.tablesorter-headerRow');

                // حذف العناصر الموجودة في المصفوفة
                foreach($classes_to_remove as $class) {
                    foreach($html->find($class) as $element) {
                        $element->outertext = ''; // حذف العنصر بأكمله
                    }
                }

                // Search for the text "Players in Server" and remove it
                $body = $html->find('body', 0);
                if ($body) {
                    foreach($body->children() as $element) {
                        if(strpos($element->innertext, 'Players in Server') !== false) {
                            $element->innertext = str_replace('Players in Server', '', $element->innertext); // Remove the text
                            break; // Stop after finding the text
                        }
                    }
                }

                // Add CSS styles for better readability
                $html->find('head', 0)->innertext .= '<style>
                    body {
                        font-family: Arial, sans-serif;
                        color: #99FF99; /* Change text color to white */
                        margin: 0;
                        padding: 20px;
                    }
                </style>';

                // عرض الصفحة بعد التعديل
                echo $html;
            ?>
        </ul>
    </div>

    <div class="overlay" id="overlay">
        <div class="overlay-content">
            <img id="overlay-image" src="" alt="">
            <span class="close-btn" onclick="closeOverlay()">&times;</span>
        </div>
    </div>
    <script>
        function openOverlay(imagePath) {
            document.getElementById('overlay').style.display = 'flex';
            document.getElementById('overlay-image').src = imagePath;
        }

        function closeOverlay() {
            document.getElementById('overlay').style.display = 'none';
        }
    </script>
</body>
</html>
