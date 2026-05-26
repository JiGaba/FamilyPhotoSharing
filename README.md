# FamilyPhotoSharing
FamilyPhotoSharing jedná se o aplikaci, která vznikla v rámci diplomové práce na FAI UTB ve školním roce 2025/2026. Jedná se o aplikaci, která umožňuje sdílení fotografií v rámci rodiny. Fotografie jsou ukládány v šifrované podobě  za pomoci hybridního šifrování. Aplikace se dělí na serverovou Docker aplikaci a nativní mobilní iOS aplikaci.

## Vygenerování self-signed certifikátů

**Windows**

otevřít power shell jako administrátor

1. Nainstaluj Chocolatey následujícím příkaz:
Set-ExecutionPolicy Bypass -Scope Process -Force; `
[System.Net.ServicePointManager]::SecurityProtocol = `
[System.Net.ServicePointManager]::SecurityProtocol -bor 3072; `
iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

2. Nainstaluj mkcert následujícím příkazem:
choco install mkcert -y

3. Vytvoř lokální autoritu:
mkcert -install

4. Vytvoř certifikáty
ideálně vytvořit localhost pro spuštění v rámci serveru a serverovou pro produkci
nejdříve je nutné zjistit IP adresu serveru nebo zařízení kde bude nainstalována Docker aplikace
poté tímto příkazem vytvořit certifikát pro localhost a server (192.168.0.1 nahradit IP adresou serveru):
mkcert localhost 192.168.0.1

5. Cesta ke kořenovému certifikátu
tento certifikát je nutné společně s certifikátem vygenerovaným v předchozím kroku předat na iOS zařízení
kořenový certifikát je nutné na iOS zařízení nainstalovat a potvrdit důvěryhodnost
poté je nutné nainstalovat a serverový certifikát
C:\Users\<username>\AppData\Local\mkcert\

**MAC**

otevřít terminál

1. Nainstaluj Homebrew:
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

2. Nainstaluj mkcert:
brew install mkcert

pokud by nebyl Homebrew přidaný do path spusťte tento příkaz, kde username nahradíte svým jménem v souborovém systému:
echo >> /Users/username/.zprofile
echo 'eval "$(/opt/homebrew/bin/brew shellenv zsh)"' >> /Users/username/.zprofile
eval "$(/opt/homebrew/bin/brew shellenv zsh)"

3. Vytvoř lokální CA:
mkcert -install

4. vytvoř certifikáty
ideálně vytvořit localhost pro spuštění v rámci serveru a serverovou pro produkci
nejdříve je nutné zjistit IP adresu serveru nebo zařízení kde bude nainstalována Docker aplikace
poté tímto příkazem vytvořit certifikát pro localhost a server (192.168.0.1 nahradit IP adresou serveru)
mkcert localhost 192.168.0.1

5. Cesta ke kořenovému certifikátu
tento certifikát je nutné společně s certifikátem vygenerovaným v předchozím kroku předat na iOS zařízení
kořenový certifikát je nutné na iOS zařízení naisntalovat a potvrdit důvěryhodnost
poté je nutné naisntalovat a serverový certifikát
pozor Library je skrytá složka a je nutné ji zobrazit
/Users/username/Library/Application Support/mkcert/rootCA.pem

**Serverový sertifikát vygenerovaný v kroku 4 je nutné nainstalovat na všechny PC/MAC/Linux klientské PC**

**Na Macu je nutné Nainstalovat Rosseta2 a následně ji i povolit v Docker desktop**

## Instalace Docker aplikace

1. Stáhnout repozitář z GitHub DP_FamilyPhotoSharing nebo zkopírovat tuto složku ze souboru prilohy.zip
2. Do DP_FamilyPhotoSharing/nginx/cert nakopírovat serverové certifikáty
3. Pokud se certifikáty jmenují jinak než localhost+1.pem a localhost+1-key.pem, tak je nutné je přejmenovat v souboru docker-compose.yml a také v souboru /nginx/nging.conf
4. V souboru DP_FamilyPhotoSharing/docker-compose.yml změnit cestu k souborovému systému C:/Users/gabaj/Documents/DockerTemp:/app/files
5. Otevřít terminál ve složce projektu kde se nachází docker-compose.yml
6. Spustit příkaz: docker compose up -d --build
7. Ve složce DP_FamilyPhotoSharing se nacházá soubor data.json, v tomto soubrou je uložen šifrovací klíč (tento klíč si můžete změnit)
8. Instalace šifrovacího klíče, bez správně vloženého klíče nejde aplikace spustit!!!

Instalace klíče WIN:
curl.exe -H "X-Vault-Token: s3cR3t-VAuLt-r00t-T0k3n-9f2b1c7e4a9d6f3b8e1d4c7a" -H "Content-Type: application/json" -X POST -d "@data.json" http://localhost:8200/v1/secret/data/myapp

Ověření klíče WIN:
curl.exe -H "X-Vault-Token: s3cR3t-VAuLt-r00t-T0k3n-9f2b1c7e4a9d6f3b8e1d4c7a" http://localhost:8200/v1/secret/data/myapp

Instalace klíče MAC:
curl -H "X-Vault-Token: s3cR3t-VAuLt-r00t-T0k3n-9f2b1c7e4a9d6f3b8e1d4c7a" \
     -H "Content-Type: application/json" \
     -X POST \
     -d @data.json \
     http://localhost:8200/v1/secret/data/myapp

Ověření klíče MAC:
curl -H "X-Vault-Token: s3cR3t-VAuLt-r00t-T0k3n-9f2b1c7e4a9d6f3b8e1d4c7a" \
     http://localhost:8200/v1/secret/data/myapp

Aplikace je dostupná na 
https://localhost
nebo
https://ip_adresa_serveru

**Poznámky**
V docker-compose.yml lze v případě kolize změnit název sítě nebo portů.
Dále je možné změnit Token k přístupu k Vault, ten je možné změnit v docker-compose.yml, poté je nutné provádět všechny příkazy pro vložení klíčů s tímto tokenem. Dále je nutné tento token změnit i v souboru DP_FamilyPhotoSharing/FamilyPhotoSharing/appsetting.json
V docker-compose.yml lze změnit i heslo k MSSQL databázi, v případě změny je nutné změnit i connections stringy

##Dependencies & Licenses
Dapper – Apache 2.0
SkiaSharp – MIT
Bootstrap – MIT
Bootstrap Icons – MIT
jQuery – MIT
