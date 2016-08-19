#!/bin/bash
rm -rf .projekt
url="https://github.com/fsprojects/Projekt"
curl -L "$url/releases/download/0.0.4/Projekt.zip" -o temp.zip
unzip temp.zip -d .projekt
rm temp.zip
