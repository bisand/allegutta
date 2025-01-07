#! /bin/bash

sudo apt update
sudo sudo apt install -y curl software-properties-common apt-transport-https ca-certificates wget gnupg
sudo curl -fSsL https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor | sudo tee /usr/share/keyrings/google-chrome.gpg > /dev/null
sudo echo deb [arch=amd64 signed-by=/usr/share/keyrings/google-chrome.gpg] http://dl.google.com/linux/chrome/deb/ stable main | sudo tee /etc/apt/sources.list.d/google-chrome.list
sudo apt update
sudo apt install -y google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf libxss1 --no-install-recommends
sudo apt install -y mariadb-server iputils-ping
sudo rm -rf /var/lib/apt/lists/*

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

dotnet dev-certs https --trust

exit 0
