#! /bin/bash

sudo service mariadb start

newUser='allegutta'
newDbPassword='password'
newDb='allegutta'
host=localhost
#host='%'

# MySQL 8 and higher versions
commands="CREATE DATABASE \`${newDb}\`;CREATE USER '${newUser}'@'${host}' IDENTIFIED BY '${newDbPassword}';GRANT USAGE ON *.* TO '${newUser}'@'${host}';GRANT ALL ON \`${newDb}\`.* TO '${newUser}'@'${host}';FLUSH PRIVILEGES;"

echo "${commands}" | sudo /usr/bin/mysql -u root

if [ -f .env ]; then
    export $(grep -v '^#' .env | xargs)
    sudo mysql -u root -e "SET PASSWORD FOR 'root'@'localhost' = PASSWORD('${MYSQL_ROOT_PASSWORD}'); FLUSH PRIVILEGES;"
    sudo mysql -u root -e "SET PASSWORD FOR '${MYSQL_USER}'@'localhost' = PASSWORD('${MYSQL_PASSWORD}'); FLUSH PRIVILEGES;"
fi
