<?php

	$servername = "localhost";
	$server_username = "root";
	$server_password = "";
	$dbName = "accounts";
	
	$username = $_POST["usernamePost"];
	$guid = $_POST["guidPost"];
	$levelFail = $_POST["levelFailPost"];
	
	$conn = new mysqli($servername,$server_username,$server_password,$dbName);
	
	if(!$conn){
		die("Connection Failed.". mysqli_connect_error());
	}
	
	$sql = "INSERT INTO sessionstate (username, guid, levelFailInARow) VALUES ('".$username."','".$guid."','".$levelFail."')";
	$result = mysqli_query($conn, $sql);
	
	if(!result) echo "there was an error.";
	else echo "Registration completed.";
?>