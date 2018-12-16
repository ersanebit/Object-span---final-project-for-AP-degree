<?php

	$servername = "localhost";
	$server_username = "root";
	$server_password = "";
	$dbName = "accounts";
	
	$username = $_POST["usernamePost"];
	$solution = $_POST["solutionPost"];
	$input = $_POST["inputPost"];
	$levelSuccess = $_POST["levelPost"];
	$gameMode = $_POST["gamePost"];
	$startedTimeStamp = $_POST["startedPost"];
	$endedTimeStamp = $_POST["endedPost"];
	
	$conn = new mysqli($servername,$server_username,$server_password,$dbName);
	
	if(!$conn){
		die("Connection Failed.". mysqli_connect_error());
	}
	
	$sql = "INSERT INTO levelstate (user,solution,input,levelSuccess,gameMode,startedTimeStamp,endedTimeStamp)
	VALUES ('".$username."','".$solution."','".$input."','".$levelSuccess."','".$gameMode."','".$startedTimeStamp."','".$endedTimeStamp."')"; 

	$result = mysqli_query($conn, $sql);
	
	if(!result) echo "there was an error.";
	else echo "Registration completed.";

?>