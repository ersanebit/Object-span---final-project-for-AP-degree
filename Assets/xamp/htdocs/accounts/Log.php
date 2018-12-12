<?php
	$servername = "localhost";
	$server_username = "root";
	$server_password = "";
	$dbName = "accounts";
	
	$username = $_POST["usernamePost"];
	$password = $_POST["passwordPost"];
	
	$conn = new mysqli($servername,$server_username,$server_password,$dbName);
	
	if(!$conn){
		die("Connection Failed.". mysqli_connect_error());
	}
	
	$sql = "SELECT password FROM account WHERE username = '".$username."' ";
	$result = mysqli_query($conn, $sql);
	
	if(mysqli_num_rows($result) > 0){
		while($row = mysqli_fetch_assoc($result)){
			if($row['password'] == $password){
				echo "login success";
			}else{
				echo "password incorrect";
			}
		}
	}else{
		echo "user not found";
	}
?>