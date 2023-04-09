echo "sleeping"
sleep 20s
echo "!!!!!!!!!!!!!!!!!!!!!!!!!!!!running set up script..."
/opt/mssql-tools/bin/sqlcmd -S "localhost" -U sa -P "${SA_PASSWORD}" -d master -i /db_init/db-init.sql

