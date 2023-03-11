docker pull mcr.microsoft.com/azure-sql-edge

docker run -p 1433:1433 -e "MSSQL_SA_PASSWORD=Admin123" -e "ACCEPT_EULA=Y" -e "MSSQL_PID=Developer" -d --name sqlpoc mcr.microsoft.com/azure-sql-edge

./setup-db.sh