sqlcmd -S localhost -U SA -Q "BACKUP DATABASE [demodb] TO DISK = N'/var/opt/mssql/data/demodb.bak' WITH NOFORMAT, NOINIT, NAME = 'demodb-full', SKIP, NOREWIND, NOUNLOAD, STATS = 10"

sqlcmd -S SERVERNAME\INSTANCE_NAME -i C:\path\mysqlfile.sql -o C:\path\output_file.txt
