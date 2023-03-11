docker cp "./db/sql/setup.sql" sqlpoc:/setup.sql

for i in {1..50};
    do
        docker exec sqlpoc /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin123 -i "/setup.sql"
        if [ $? -eq 0 ]
        then
            echo "setup.sql completed"
            break
        else
            echo "not ready yet..."
            sleep 1
        fi
    done