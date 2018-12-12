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
	
	$sql = "INSERT INTO account (username, password) VALUES ('".$username."','".$password."')";
	$result = mysqli_query($conn, $sql);
	
	if(!result) echo "there was an error.";
	else echo "Registration completed.";
?>