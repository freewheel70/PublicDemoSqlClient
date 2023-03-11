docker pull mcr.microsoft.com/azure-sql-edge

Write-Output "Running container ..."
docker run -p 1433:1433 -e "MSSQL_SA_PASSWORD=Admin123" -e "ACCEPT_EULA=Y" -e "MSSQL_PID=Developer" -d --name sqlpoc mcr.microsoft.com/azure-sql-edge

Write-Output "Copy setup sql file into container ..."
docker cp "./db/sql/setup.sql" sqlpoc:/setup.sql


Write-Output "Start setup DB ..."
$counter = 0
$maxRetries = 5
DO {
    $message = $counter.ToString() + ": trying ..."
    Write-Output $message
    docker exec sqlpoc /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin123 -i "/setup.sql"
    start-sleep -Seconds 2
} while(!$? -and $counter -lt $maxRetries) 

if (!$?) {
  Write-Output "Failed after all retries"
}else {
    Write-Output "Setup DB completed!"
}