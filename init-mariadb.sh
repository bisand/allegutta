#! /bin/sh
if [ -f .env ]; then
    export $(grep -v '^#' .env | xargs)
    sudo mysql -u root -e "SET PASSWORD FOR 'root'@'localhost' = PASSWORD('${MYSQL_ROOT_PASSWORD}'); FLUSH PRIVILEGES;"
    sudo mysql -u root -e "SET PASSWORD FOR '${MYSQL_USER}'@'localhost' = PASSWORD('${MYSQL_PASSWORD}'); FLUSH PRIVILEGES;"
fi
