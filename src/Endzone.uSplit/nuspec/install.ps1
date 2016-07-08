param($installPath, $toolsPath, $package, $project)

$projectFullName = $project.FullName
$debugString = "uSplit install.ps1 executing for " + $projectFullName
Write-Host $debugString
 
$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $projectFullName
$projectDirectory = $fileInfo.DirectoryName

$sourceDirectory = "$installPath\log4net"
 
$destinationDirectory = "$projectDirectory\bin\log4net"
 
if(test-path $sourceDirectory -pathtype container)
{
 Write-Host "Copying files from $sourceDirectory to $destinationDirectory"
 robocopy $sourceDirectory $destinationDirectory /s /xo
}
 
$debugString = "uSplit install.ps1 complete" + $projectFullName
Write-Host $debugString