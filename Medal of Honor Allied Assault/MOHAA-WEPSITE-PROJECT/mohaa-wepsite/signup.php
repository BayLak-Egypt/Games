<?php
// Include database connection
include 'db_connect.php';

// Initialize message variables
$signup_message = "";

// Process signup form submission
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $username = $_POST['username'];
    $email = $_POST['email'];
    $password = $_POST['password'];

    // Check if user already exists in database
    $check_query = "SELECT * FROM users WHERE username='$username'";
    $check_result = mysqli_query($conn, $check_query);

    if (mysqli_num_rows($check_result) > 0) {
        $signup_message = "Account with this username already exists. Please choose a different username.";
    } else {
        // Insert user data into database
        $sql = "INSERT INTO users (username, email, password) VALUES ('$username', '$email', '$password')";

        if (mysqli_query($conn, $sql)) {
            $signup_message = "Account created successfully!";
        } else {
            $signup_message = "Error: " . $sql . "<br>" . mysqli_error($conn);
        }
    }
}
?>


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-u+j8AzCz5dUKYfUZ2gqRfTBoOg0rNq6lZZT8tJCNObtwXLx1LpPt4X2jJcuFjOpq5J1q5z36oMn2Q6uxvj7yLw==" crossorigin="anonymous" referrerpolicy="no-referrer" />
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

        /* تحديث الرموز ل Font Awesome */
        .file-icon::before {
            content: "\f15c"; /* رمز الصندوق للملف */
            margin-right: 8px;
        }

        .download-icon::before {
            content: "\f019"; /* رمز السهم للتحميل */
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
            qdom_error   top: 10px;
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
    margin-bottom: 5px; /* تزيين المسافة بين الفقرات */
}

.item .download-btn {
    margin-top: auto; /* لوضع زر التحميل في الجزء السفلي للعنصر */
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

.file-icon {
    /* اسلوب تنسيق الرمز الذي يرمز إلى تنزيل الملف */
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
  input[type="text"],
        input[type="password"],
        input[type="email"],
		input[type="submit"] {
            width: 100%;
            padding: 10px;
            margin-bottom: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
            box-sizing: border-box;
        }
        input[type="submit"] {
            background-color: #007bff;
            color: #fff;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        input[type="submit"]:hover {
            background-color: #0056b3;
        }

        .signup-link {
            margin-top: 15px;
            font-size: 14px;
            color: #fff;
        }

        .signup-link a {
            color: #007bff;
            text-decoration: none;
        }

        .signup-link a:hover {
            text-decoration: underline;
        }
		 .container {
            max-width: 400px;
            margin: 0 auto;
            background-color: #000;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
        }
		 body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            text-align: center;
            background-color: #1e1e1e;
            color: #fff;
        }
		.message {
            color: #f00;
            font-size: 14px;
            margin-bottom: 10px;
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


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login</title>
</head>
<body>

<div class="container">
        <!-- عرض رسالة الإشعار -->
        <?php if (!empty($signup_message)) { ?>
            <span class="message"><?php echo $signup_message; ?></span>
        <?php } ?>

        <!-- Login form -->
        <form id="loginForm" action="" method="post">
           <input type="text" name="username" placeholder="Username" required><br>
            <input type="email" name="email" placeholder="Email" required><br>
            <input type="password" name="password" placeholder="Password" required><br>
			
            <input type="submit" value="Sign Up">
        </form>

        <!-- Link to register a new account -->
        <p class="signup-link">I have account <a href="login.php">Login</a></p>
    </div>
	
</body>
</html>
