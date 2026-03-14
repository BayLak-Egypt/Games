<?php
session_start();

// Include database connection file
include_once "db_connect.php";

if (!isset($_SESSION['username'])) {
    header("Location: login.php");
    exit();
}

?>

<!DOCTYPE html>
<html lang="ar">
<head>
    <meta charset="UTF-8">
    <title>Profile</title>
    <style>
      body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
		    text-align: center;
            align-items: center;
            justify-content: center;
            background-color: #1e1e1e; /* لون خلفية الصفحة الداكن */
            color: #fff; /* لون النص */
        }

        .container {
            display: flex;
            max-width: 1600px;
            margin: 0 auto;
            background-color: #000; /* لون خلفية العنصر الرئيسي */
            padding: 20px;
            border-radius: 8px;
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
        .nav {
            flex: 0 0 auto;
            width: 250px; /* زيادة عرض القائمة */
            background: #3a3a3a; /* خلفية داكنة للقائمة */
            padding: 10px;
            border-radius: 10px;
            height: 100%; /* جعل القائمة تأخذ كامل ارتفاع الحاوية */
            margin-right: 20px; /* زيادة المسافة بين القائمة والمحتوى */
        }

        .nav ul {
            list-style-type: none;
            padding: 0;
        }
        .nav li {
            margin-bottom: 15px; /* زيادة المسافة بين العناصر */
        }
        .nav a {
            display: block;
            color: #ffffff;
            text-decoration: none;
            padding: 15px; /* زيادة مساحة النقر */
            border-radius: 5px;
            background: #4a4a4a;
            font-size: 18px; /* زيادة حجم الخط */
        }
        .nav a:hover {
            background: #565656;
        }

        .content {
            flex: 1 1 auto;
            padding: 20px; /* زيادة padding */
            overflow-y: auto; /* للسماح بالتمرير عند الحاجة */
            background: #2c2c2c; /* خلفية داكنة للمحتوى */
            border-radius: 10px;
        }
        .content h2 {
            color: #ffffff; /* لون العنوان */
            font-size: 24px; /* زيادة حجم الخط للعناوين */
        }
        .content p {
            font-size: 16px; /* زيادة حجم الخط للنص */
            color: #dcdcdc; /* لون النص */
        }
          .container1 {
            max-width: 1600px;
            margin: 0 auto;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            text-align: center; /* محاذاة النص في الوسط */
            animation: fadeIn 0.5s ease-out forwards;
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-50px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .success-message {
            color: #2ecc71; /* لون النجاح */
            font-weight: bold;
            font-size: 24px; /* حجم الخط */
            margin-bottom: 20px; /* المسافة بين الرسالة وبقية المحتوى */
            display: none; /* يكون غير مرئي بشكل افتراضي */
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
            transform: translate(2%, -62%);
            color: #fff;
        }
    </style>
</head>
<body>
<div class="container1">
    <?php
    // التحقق مما إذا تم نقل المستخدم بنجاح من صفحة التحفظ
    if (isset($_GET['success']) && $_GET['success'] === 'true') {
        echo '<p class="success-message">تم الحفظ بنجاح!</p>';
        // يمكنك تحديد المدة التي ترغب في أن تظهر فيها الرسالة قبل أن تختفي
        echo '<meta http-equiv="refresh" content="5;url=profile.php" />';
    }
    ?>
</div>

<div class="banner">
  <img src="images/bannar.jpg" alt="Banner" class="banner-img">
  <div class="banner-overlay">
    <tst>
        <ul>
          <li> 
		            <a href="maps.php">Maps</a></li>
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
    <div class="nav">
        <ul>
            <li1><a href="#" onclick="loadPage('account')">Account</a></li1>
	        <li1><a href="#" onclick="loadPage('UploadData')">Upload</a></li1>
			            <li1><a href="#" onclick="loadPage('toolsData')">Upload for Tools</a></li1>
            <li1><a href="#" onclick="loadPage('uploaded')">Files that have been uploaded</a></li1>
            <li1><a href="#" onclick="loadPage('account-settings')">Account settings</a></li1>
        </ul>
    </div>

    <div class="content" id="content">
        <!-- هنا سيتم تحميل المحتوى -->
    </div>
</div>


<script>
    // دالة لعرض رسالة النجاح وإخفائها بعد مرور فترة زمنية
    function showSuccessMessage() {
        // عرض رسالة النجاح
        var successMessage = document.querySelector('.success-message');
        successMessage.style.display = 'block';

        // تأخير إخفاء رسالة النجاح بعد 3 ثواني (3000 مللي ثانية)
        setTimeout(function() {
            successMessage.style.display = 'none';
        }, 3000); // 3000 مللي ثانية = 3 ثواني
    }

    // دالة لتحميل الصفحة المطلوبة بواسطة Ajax
    function loadPage(page) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function() {
            if (this.readyState == 4 && this.status == 200) {
                document.getElementById("content").innerHTML = this.responseText;
                showSuccessMessage(); // عرض رسالة النجاح بعد تحميل الصفحة
            }
        };
        xhttp.open("GET", page + ".php", true);
        xhttp.send();
    }

    // تحميل الصفحة الافتراضية عند تحميل الصفحة الأولى
    window.onload = function() {
        loadPage('account');
    };
</script>


</body>
</html>
