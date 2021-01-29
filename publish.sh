#!/usr/bin/env sh

# abort on errors
set -e

cd Assets/UnityGoogleDrive

git init
git add -A
git add -f ThirdParty/JsonNet-Lite/UnityGoogleDrive.Newtonsoft.Json.dll
git reset -- Resources/
git reset -- Resources.meta
git commit -m 'publish'
git push -f git@github.com:Elringus/UnityGoogleDrive.git master:package

cd -